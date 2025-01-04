using Azure.Core;
using Azure.Identity;
using Json.Path;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Alertmanager.Receiver.AzureDevOps;

public class AlertProcessor : IAlertProcessor
{
    private readonly ILogger<AlertProcessor> _logger;
    private readonly Settings _settings;
    private readonly Instrumentation _meters;
    private DefaultAzureCredential? azureCredential;

    public AlertProcessor(ILogger<AlertProcessor> logger, Settings settings, Instrumentation meters)
    {
        _logger = logger;
        _settings = settings;
        _meters = meters;
    }

    public async Task ProcessAlert(AlertmanagerPayload payload)
    {
        var json = JsonSerializer.Serialize(payload, typeof(AlertmanagerPayload), JSContext.Default);
        var jsonNode = (JsonNode)JsonSerializer.Deserialize(json, typeof(JsonNode), JSContext.Default)!;

        _logger.LogDebug("Processing Alert: {payload}", json);

        var connection = await VssConnection();

        var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

        if (payload.Status == "firing")
        {
            // Create new WorkItem
            var document = GenerateWorkItem(payload, jsonNode, _settings.NewWorkItemFields, true);

            _logger.LogDebug("Prepared Payload: {payload}", JsonSerializer.Serialize(document, typeof(JsonPatchDocument), JSContext.Default));

            var workItem = await workItemTrackingClient.CreateWorkItemAsync(document, _settings.Project, _settings.WorkItemType);
            _meters.AlertCounter.Add(1);
            _logger.LogInformation("Created WorkItem: {id} - {title}", workItem.Id, workItem.Fields["System.Title"]);
        }
        else
        {
            // Resolved
            // Get existing Ticket Id
            var searchResults = await workItemTrackingClient.QueryByWiqlAsync(new Wiql { Query = $"SELECT [System.Id] FROM WorkItems WHERE [System.Tags] CONTAINS 'Fingerprint:{payload.Alerts[0].Fingerprint}'" });

            // Patch Ticket with Resolved status
            if (searchResults.WorkItems.Any())
            {
                var workItemRef = searchResults.WorkItems.First();

                var document = GenerateWorkItem(payload, jsonNode, _settings.ResolvedWorkItemFields, false);

                _logger.LogDebug("Prepared Payload: {payload}", JsonSerializer.Serialize(document, typeof(JsonPatchDocument), JSContext.Default));

                var workItem = await workItemTrackingClient.UpdateWorkItemAsync(document, workItemRef.Id);
                _meters.ResolvedAlertCounter.Add(1);
                _logger.LogInformation("Resolved WorkItem: {id} - {title}", workItem.Id, workItem.Fields["System.Title"]);
            }
        }
    }

    private async Task<VssConnection> VssConnection()
    {
        VssCredentials creds;

        if (!string.IsNullOrEmpty(_settings.PAT))
        {
            creds = new VssBasicCredential(string.Empty, _settings.PAT);
        }
        else
        {
            azureCredential ??= new DefaultAzureCredential(new DefaultAzureCredentialOptions()
            {
                ExcludeAzureCliCredential = true,
                ExcludeAzureDeveloperCliCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeVisualStudioCredential = true,
            });

            try
            {
                var accessToken = await azureCredential.GetTokenAsync(new TokenRequestContext(["499b84ac-1321-427f-aa17-267ca6975798/.default"]));
                creds = new VssAadCredential(new VssAadToken(accessToken.TokenType, accessToken.Token));
            }
            catch (MsalServiceException ex)
            {
                _logger.LogError(ex, "Error Acquiring Token");
                throw;
            }
        }

        var connection = new VssConnection(new Uri($"https://dev.azure.com/{_settings.Organization}"), creds);

        return connection;
    }

    private JsonPatchDocument GenerateWorkItem(AlertmanagerPayload payload, JsonNode jsonNode, List<Field> fields, bool addFingerprint)
    {
        var document = new JsonPatchDocument();

        foreach (var field in fields)
        {
            var item = new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/" + field.ReferenceName
            };

            if (!string.IsNullOrEmpty(field.DirectValue))
            {
                item.Value = field.DirectValue;
            }
            else if (!string.IsNullOrEmpty(field.Format))
            {
                var parameters = new List<string>();

                foreach (var path in field.JSONPaths)
                {
                    var jsonPath = JsonPath.Parse(path);

                    var results = jsonPath.Evaluate(jsonNode);

                    if (results is null || results.Matches is null || results.Matches.Count == 0 || results.Matches[0].Value is null)
                    {
                        var message = $"Field: {field.ReferenceName} JSONPath: {path} Warning: JSONPath did not find a match.";
                        parameters.Add(message);
                        _logger.LogWarning(message);

                        continue;
                    }

                    if (results.Matches[0].Value is JsonArray array)
                    {
                        parameters.Add(JsonSerializer.Serialize(array, typeof(JsonArray), JSContext.Default));
                    }
                    else
                    {
                        parameters.Add(results.Matches[0].Value!.ToString());
                    }
                }

                item.Value = string.Format(field.Format, parameters.ToArray());
            }

            document.Add(item);
        }

        // Add Tag with Fingerprint
        if (addFingerprint)
        {
            var tags = document.FirstOrDefault(x => x.Path == "/fields/System.Tags");

            if (tags != null)
            {
                tags.Value = tags.Value + "; Fingerprint:" + payload.Alerts[0].Fingerprint;
            }
            else
            {
                var tag = new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Tags",
                    Value = "Fingerprint:" + payload.Alerts[0].Fingerprint
                };

                document.Add(tag);
            }
        }

        return document;
    }
}
