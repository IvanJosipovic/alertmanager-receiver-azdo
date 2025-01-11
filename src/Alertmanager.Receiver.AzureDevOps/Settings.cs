namespace Alertmanager.Receiver.AzureDevOps;

public class Settings
{
    public LogLevel LogLevel { get; set; }

    public LogFormat LogFormat { get; set; }

    public string Organization { get; set; } = null!;

    public string Project { get; set; } = null!;

    public string WorkItemType { get; set; } = null!;

    public List<Field> ResolvedWorkItemFields { get; set; } = null!;

    public List<Field> NewWorkItemFields { get; set; } = null!;

    public Authentication Authentication { get; set; } = null!;
}

public class Authentication
{
    public string PAT { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}

public enum LogFormat
{
    Simple,
    JSON,
}

public class Field
{
    public string ReferenceName { get; set; } = null!;

    public string[] JSONPaths { get; set; } = null!;

    public string Format { get; set; } = null!;

    public string DirectValue { get; set; } = null!;
}
