using CargoTrack.DTO.DashboardDtos;

namespace CargoTrack.Business.Abstract;

public interface IManagerDashboardService
{
    Task<ManagerDashboardDto> GetAsync(Guid branchId);
}
