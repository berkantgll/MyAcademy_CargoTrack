
using CargoTrack.DTO.DashboardDtos;

namespace CargoTrack.Business.Abstract;

public interface IUserDashboardService
{
    Task<UserDashboardDto> GetAsync(Guid customerId);
}
