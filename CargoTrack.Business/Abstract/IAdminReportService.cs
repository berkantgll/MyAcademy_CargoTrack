using CargoTrack.DTO.ReportDtos;

namespace CargoTrack.Business.Abstract;

public interface IAdminReportService
{
    Task<AdminReportDto> GetReportAsync();
}