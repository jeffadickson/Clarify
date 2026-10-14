using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddElsa();
await using var provider = services.BuildServiceProvider();

var workflow = new Sequence
{
    Activities =
    {
        new WriteLine("Clarify Elsa 3.8 runtime smoke: started"),
        new WriteLine("Clarify Elsa 3.8 runtime smoke: completed")
    }
};
var runner = provider.GetRequiredService<IWorkflowRunner>();
await runner.RunAsync(workflow);
Console.WriteLine("ELSA_SMOKE_COMPLETE");
