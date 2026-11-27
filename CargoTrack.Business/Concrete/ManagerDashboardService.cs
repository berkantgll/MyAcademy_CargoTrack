using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.DashboardDtos;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class ManagerDashboardService : IManagerDashboardService
{
    private readonly IManagerDashboardRepository _repository;

    public ManagerDashboardService(
        IManagerDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<ManagerDashboardDto> GetAsync(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("Geçersiz şube.");

        var data = await _repository.GetAsync(branchId);

        return data.Adapt<ManagerDashboardDto>();
    }
}
