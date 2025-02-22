using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Alertmanager.Receiver.AzureDevOps;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Add(JSContext.Default);
        });

        var settings = builder.Configuration.GetSection("Settings").Get<Settings>()!;

        new SettingsValidator().ValidateAndThrow(settings);

        builder.Services.AddSingleton(settings);

        builder.Services.AddSingleton<IAlertProcessor, AlertProcessor>();
        builder.Services.AddSingleton<Instrumentation>();

        if (settings.LogFormat == LogFormat.JSON)
        {
            builder.Logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = false;
                options.TimestampFormat = "HH:mm:ss";
            });
        }

        builder.Logging.AddFilter("Default", settings.LogLevel);
        builder.Logging.AddFilter("Alertmanager", settings.LogLevel);
        builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Error);

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: "alertmanager-receiver-azdo"))
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddEventCountersInstrumentation(c =>
                    {
                        c.AddEventSources(
                            "Microsoft.AspNetCore.Hosting",
                            "Microsoft-AspNetCore-Server-Kestrel",
                            "System.Net.Http",
                            "System.Net.Sockets");
                    })
                    .AddView("request-duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                    })
                    .AddMeter(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "alertmanager_receiver_azdo"
                    )
                    .AddPrometheusExporter();
            });

        builder.Services.AddMetrics();
        builder.Services.AddHealthChecks();
        builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);

        var app = builder.Build();
        app.Logger.LogInformation("Starting Application");
        app.UseForwardedHeaders();
        app.MapPrometheusScrapingEndpoint();
        app.MapHealthChecks("/health");

        app.MapPost("/alert", async ([FromServices] IAlertProcessor processor, [FromBody] AlertmanagerPayload payload) =>
        {
            await processor.ProcessAlert(payload);
        });

        app.Run();
    }
}

[JsonSerializable(typeof(AlertmanagerPayload))]
[JsonSerializable(typeof(JsonArray))]
[JsonSerializable(typeof(JsonNode))]
[JsonSerializable(typeof(JsonPatchDocument))]
public partial class JSContext : JsonSerializerContext
{
}