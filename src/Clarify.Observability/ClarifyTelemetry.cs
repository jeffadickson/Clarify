using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Clarify.Observability;

/// <summary>Shared instrumentation names used by generated applications and Studio.</summary>
public static class ClarifyTelemetry
{
    public const string SourceName = "Clarify.Application";
    public static readonly ActivitySource Source = new(SourceName);
    public static readonly Meter Meter = new(SourceName);
    public static readonly Counter<long> Saves = Meter.CreateCounter<long>("clarify.form.saves");
    public static readonly Counter<long> SaveFailures = Meter.CreateCounter<long>("clarify.form.save.failures");
    public static readonly Histogram<double> SaveDuration = Meter.CreateHistogram<double>("clarify.form.save.duration","ms");

    /// <summary>Records operational metadata without customer names, form values or SQL parameters.</summary>
    public static Activity? StartOperation(string operation, string entity)
    {
        var activity = Source.StartActivity(operation, ActivityKind.Internal);
        activity?.SetTag("clarify.entity", entity);
        return activity;
    }
}

/// <summary>
/// Opt-in OTLP export. With no endpoint, the app remains operational offline.
/// Instrumentation never requires a Collector to be available.
/// </summary>
public static class ObservabilityRegistration
{
    public static IServiceCollection AddClarifyObservability(
        this IServiceCollection services, IConfiguration config)
    {
        var serviceName = config["Clarify:Observability:ServiceName"] ?? "Clarify";
        var endpoint = config["Clarify:Observability:OtlpEndpoint"];
        if (!string.IsNullOrWhiteSpace(endpoint) &&
            (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) ||
             uri.Scheme is not ("http" or "https")))
            throw new InvalidOperationException("Invalid OTLP endpoint.");
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t =>
            {
                t.AddSource(ClarifyTelemetry.SourceName)
                 .AddAspNetCoreInstrumentation(o =>
                 {
                     o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
                 })
                 .AddHttpClientInstrumentation();
                if (!string.IsNullOrWhiteSpace(endpoint))
                    t.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            })
            .WithMetrics(m =>
            {
                m.AddMeter(ClarifyTelemetry.SourceName)
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation();
                if (!string.IsNullOrWhiteSpace(endpoint))
                    m.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            });
        return services;
    }
}