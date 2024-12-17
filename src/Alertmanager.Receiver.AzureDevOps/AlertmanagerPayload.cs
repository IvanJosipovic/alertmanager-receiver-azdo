using System.Text.Json.Serialization;

namespace Alertmanager.Receiver.AzureDevOps;

// https://prometheus.io/docs/alerting/latest/configuration/#webhook_config
public class AlertmanagerPayload
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("groupKey")]
    public string GroupKey { get; set; }

    [JsonPropertyName("truncatedAlerts")]
    public int TruncatedAlerts { get; set; }

    // resolved or firing
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("receiver")]
    public string Receiver { get; set; }

    [JsonPropertyName("groupLabels")]
    public Dictionary<string, string> GroupLabels { get; set; }

    [JsonPropertyName("commonLabels")]
    public Dictionary<string, string> CommonLabels { get; set; }

    [JsonPropertyName("commonAnnotations")]
    public Dictionary<string, string> CommonAnnotations { get; set; }

    [JsonPropertyName("externalURL")]
    public string ExternalURL { get; set; }

    [JsonPropertyName("alerts")]
    public List<Alert> Alerts { get; set; }
}

public class Alert
{
    // resolved or firing
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string> Labels { get; set; }

    [JsonPropertyName("annotations")]
    public Dictionary<string, string> Annotations { get; set; }

    [JsonPropertyName("startsAt")]
    public DateTime StartsAt { get; set; }

    [JsonPropertyName("endsAt")]
    public DateTime EndsAt { get; set; }

    [JsonPropertyName("generatorURL")]
    public string GeneratorURL { get; set; }

    [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; }
}
