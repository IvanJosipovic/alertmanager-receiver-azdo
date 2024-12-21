using System.Diagnostics.Metrics;

namespace Alertmanager.Receiver.AzureDevOps;

public class Instrumentation
{
    public Counter<long> AlertCounter { get; private set; }
    public Counter<long> ResolvedAlertCounter { get; private set; }


    public Instrumentation(IMeterFactory meterFactory)
    {
        const string prefix = "alertmanager_receiver_azdo";
        var meter = meterFactory.Create(prefix);

        AlertCounter = meter.CreateCounter<long>(prefix + "_alerts", description: "Number of Alerts created.");
        ResolvedAlertCounter = meter.CreateCounter<long>(prefix + "_alerts", description: "Number of Alerts resolved.");
    }
}
