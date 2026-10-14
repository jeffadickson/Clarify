using System.ComponentModel.DataAnnotations;
namespace Clarify.Forms.Shared;
public sealed class CustomerModel {
 public long Id { get; set; }
 [Required, StringLength(160)] public string Name { get; set; } = "";
 [EmailAddress, StringLength(254)] public string? Email { get; set; }
 [StringLength(120)] public string? City { get; set; }
}
public interface ICustomerStore {
 Task<IReadOnlyList<CustomerModel>> ListAsync(CancellationToken ct = default);
 Task<CustomerModel?> GetAsync(long id,CancellationToken ct = default);
 Task<long> SaveAsync(CustomerModel model,CancellationToken ct = default);
}
