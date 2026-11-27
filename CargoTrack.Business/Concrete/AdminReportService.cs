
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.ReportDtos;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class AdminReportService : IAdminReportService
{
    private readonly IAdminReportRepository _repository;

    public AdminReportService(IAdminReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminReportDto> GetReportAsync()
    {
        var data = await _repository.GetReportAsync();

        return data.Adapt<AdminReportDto>();
    }
}
