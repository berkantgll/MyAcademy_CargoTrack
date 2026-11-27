using CargoTrack.DataAccess.Reports;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface IUserDashboardRepository
{
    Task<UserDashboardSnapshot> GetAsync(Guid customerId);
}
