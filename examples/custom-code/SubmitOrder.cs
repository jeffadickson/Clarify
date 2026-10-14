// User-owned extension example. Clarify generator must never overwrite this file.
using Clarify.Extensibility;
namespace Clarify.Examples.CustomCode;
public sealed record SubmitOrder(long OrderId);
public interface IOrderService { Task SubmitAsync(long orderId, CancellationToken cancellationToken); }
public sealed class SubmitOrderHandler(IOrderService service): ICommandHandler<SubmitOrder> {
 public Task ExecuteAsync(SubmitOrder command, CancellationToken cancellationToken) =>
     service.SubmitAsync(command.OrderId, cancellationToken);
}
