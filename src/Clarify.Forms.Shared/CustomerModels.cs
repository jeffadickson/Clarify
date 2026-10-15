using System.ComponentModel.DataAnnotations;
namespace Clarify.Forms.Shared;
public sealed class CustomerModel {
 public long Id { get; set; }
 [Required, StringLength(160)] public string Name { get; set; } = "";
 [EmailAddress, StringLength(254)] public string? Email { get; set; }
 [StringLength(120)] public string? City { get; set; }
}
/// <summary>Bounded page returned by an asynchronous browse operation.</summary>
public sealed record CustomerPageResult(IReadOnlyList<CustomerModel> Items, int Page, int PageSize, bool HasNext);

public interface ICustomerStore {
 /// <summary>
 /// Returns at most pageSize rows; the extra database row is used only to detect
 /// whether Next is available. Cancellation is propagated to EF Core.
 /// </summary>
 Task<CustomerPageResult> PageAsync(int page, int pageSize, CancellationToken ct = default);
 Task<IReadOnlyList<CustomerModel>> ListAsync(CancellationToken ct = default);
 Task<CustomerModel?> GetAsync(long id,CancellationToken ct = default);
 Task<long> SaveAsync(CustomerModel model,CancellationToken ct = default);
}
