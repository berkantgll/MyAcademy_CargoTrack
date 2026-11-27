
using CargoTrack.DataAccess.Reports;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface IManagerDashboardRepository
{
    Task<ManagerDashboardSnapshot> GetAsync(Guid branchId);
}
