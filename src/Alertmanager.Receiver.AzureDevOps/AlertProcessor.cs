using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Text.Json;

namespace Alertmanager.Receiver.AzureDevOps;

public class AlertProcessor : IAlertProcessor
{
    private readonly ILogger<AlertProcessor> _logger;
    private readonly Settings _settings;

    public AlertProcessor(ILogger<AlertProcessor> logger, Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task ProcessAlert(AlertmanagerPayload payload)
    {
        _logger.LogInformation("Processing Alert: {payload}", JsonSerializer.Serialize(payload));

        var connection = new VssConnection(new Uri($"https://dev.azure.com/{_settings.Organization}"), new VssBasicCredential(string.Empty, _settings.PAT));

        var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

        var document = new JsonPatchDocument
        {
            new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/System.Title",
                Value = $"Alert: {payload.CommonAnnotations["summary"]}"
            },
            new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/System.Description",
                Value = $"Alert details: {payload.CommonAnnotations["description"]}"
            },
            new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/System.Tags",
                Value = "Alert"
            }
        };

        await workItemTrackingClient.CreateWorkItemAsync(document, _settings.Project, "Issue");
    }
}
