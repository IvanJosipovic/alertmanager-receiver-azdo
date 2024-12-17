namespace Alertmanager.Receiver.AzureDevOps;

public class Settings
{
    public LogLevel LogLevel { get; set; }

    public string Organization { get; set; } = null!;

    public string Project { get; set; } = null!;

    public string PAT { get; set; } = null!;
}
