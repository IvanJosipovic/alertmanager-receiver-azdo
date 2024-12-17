namespace Alertmanager.Receiver.AzureDevOps;

public class Settings
{
    public string PAT { get; set; } = null!;

    public string Organization { get; set; } = null!;

    public string Project { get; set; } = null!;
}
