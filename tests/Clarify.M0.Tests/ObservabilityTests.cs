using System.Diagnostics;
using System.Diagnostics.Metrics;
using Clarify.Observability;
using Xunit;

namespace Clarify.M0.Tests;

/// <summary>
/// Verifies instrumentation without requiring a running collector or network.
/// Use a local listener so failures indicate the instrumentation contract itself.
/// </summary>
public sealed class ObservabilityTests
{
    [Fact]
    public void StartOperationEmitsNamedSpanWithoutBusinessData()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == ClarifyTelemetry.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);
        using var span = ClarifyTelemetry.StartOperation("clarify.customer.save", "Customer");
        Assert.NotNull(span);
        Assert.Equal("Clarify.Application", span!.Source.Name);
        Assert.Equal("Customer", span.GetTagItem("clarify.entity"));
        Assert.DoesNotContain(span.Tags, tag => tag.Key.Contains("password", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void FormSaveCounterEmitsMetric()
    {
        long sum = 0;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == ClarifyTelemetry.SourceName)
                meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, state) =>
        {
            if (instrument.Name == "clarify.form.saves") sum += value;
        });
        listener.Start();
        ClarifyTelemetry.Saves.Add(1, new KeyValuePair<string, object?>("entity", "Customer"));
        Assert.Equal(1, sum);
    }
}
