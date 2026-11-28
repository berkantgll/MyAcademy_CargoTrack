
namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface IBranchUsageRepository
{
    Task<bool> HasOpenCargosAsync(Guid branchId);

    Task<bool> HasActiveAssignmentsAsync(Guid branchId);

    Task<bool> HasAnyReferencesAsync(Guid branchId);
}
