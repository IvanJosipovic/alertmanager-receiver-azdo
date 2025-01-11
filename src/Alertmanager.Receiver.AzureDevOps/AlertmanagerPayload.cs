using System.Text.Json.Serialization;

namespace Alertmanager.Receiver.AzureDevOps;

// https://prometheus.io/docs/alerting/latest/configuration/#webhook_config
public class AlertmanagerPayload
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("groupKey")]
    public string GroupKey { get; set; } = null!;

    [JsonPropertyName("truncatedAlerts")]
    public int TruncatedAlerts { get; set; }

    // resolved or firing
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("receiver")]
    public string Receiver { get; set; } = null!;

    [JsonPropertyName("groupLabels")]
    public Dictionary<string, string> GroupLabels { get; set; } = null!;

    [JsonPropertyName("commonLabels")]
    public Dictionary<string, string> CommonLabels { get; set; } = null!;

    [JsonPropertyName("commonAnnotations")]
    public Dictionary<string, string> CommonAnnotations { get; set; } = null!;

    [JsonPropertyName("externalURL")]
    public string ExternalURL { get; set; } = null!;

    [JsonPropertyName("alerts")]
    public List<Alert> Alerts { get; set; } = null!;
}

public class Alert
{
    // resolved or firing
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("labels")]
    public Dictionary<string, string> Labels { get; set; } = null!;

    [JsonPropertyName("annotations")]
    public Dictionary<string, string> Annotations { get; set; } = null!;

    [JsonPropertyName("startsAt")]
    public DateTime StartsAt { get; set; }

    [JsonPropertyName("endsAt")]
    public DateTime EndsAt { get; set; }

    [JsonPropertyName("generatorURL")]
    public string GeneratorURL { get; set; } = null!;

    [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; } = null!;
}
