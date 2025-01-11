namespace Alertmanager.Receiver.AzureDevOps;

public interface IAlertProcessor
{
    Task ProcessAlert(AlertmanagerPayload payload);
}