using CargoTrack.DataAccess.Reports;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface IAdminReportRepository
{
    Task<AdminReportSnapshot> GetReportAsync();
}