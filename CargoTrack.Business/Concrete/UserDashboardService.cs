using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.DashboardDtos;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class UserDashboardService : IUserDashboardService
{
    private readonly IUserDashboardRepository _repository;

    public UserDashboardService(
        IUserDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDashboardDto> GetAsync(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Geçersiz müşteri.");

        var data = await _repository.GetAsync(customerId);

        return data.Adapt<UserDashboardDto>();
    }
}
