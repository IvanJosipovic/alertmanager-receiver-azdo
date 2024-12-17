using Microsoft.AspNetCore.Mvc;

namespace Alertmanager.Receiver.AzureDevOps;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var settings = builder.Configuration.GetSection("Settings").Get<Settings>()!;
        builder.Services.AddSingleton(settings);

        builder.Services.AddSingleton<IAlertProcessor, AlertProcessor>();

        var app = builder.Build();

        app.MapPost("/alert", async ([FromServices] IAlertProcessor processor, [FromBody] AlertmanagerPayload payload) =>
        {
            await processor.ProcessAlert(payload);
        });

        app.Run();
    }
}
