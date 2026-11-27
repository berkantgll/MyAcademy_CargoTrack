
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Reports;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class AdminReportRepository : IAdminReportRepository
{
    private readonly CargoTrackDbContext _context;

    public AdminReportRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<AdminReportSnapshot> GetReportAsync()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var sevenDaysAgo = today.AddDays(-6);

        var cargos = _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        var result = new AdminReportSnapshot();

        result.StatusDistribution = await cargos
            .GroupBy(x => x.Status)
            .Select(g => new ReportStatusRow
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        result.TotalCargos = result.StatusDistribution
            .Sum(x => x.Count);

        result.DeliveredCargos = result.StatusDistribution
            .Where(x => x.Status == CargoStatus.Delivered)
            .Sum(x => x.Count);

        result.ReturnedCargos = result.StatusDistribution
            .Where(x => x.Status == CargoStatus.ReturnedToSender)
            .Sum(x => x.Count);

        result.DelayedCargos = await cargos.CountAsync(x =>
            x.EstimatedDeliveryDate < today &&
            x.Status != CargoStatus.Delivered &&
            x.Status != CargoStatus.ReturnedToSender);

        result.OnTimeDeliveries = await cargos.CountAsync(x =>
            x.DeliveredAt.HasValue &&
            x.DeliveredAt.Value.Date <= x.EstimatedDeliveryDate.Date);

        var averageMinutes = await cargos
            .Where(x => x.DeliveredAt.HasValue)
            .AverageAsync(x =>
                (double?)EF.Functions.DateDiffMinute(
                    x.CreatedDate,
                    x.DeliveredAt!.Value));

        result.AverageDeliveryHours =
            Math.Round((averageMinutes ?? 0) / 60, 1);

        result.ActiveBranches = await _context.Set<Branch>()
            .CountAsync(x => !x.IsDeleted && x.IsActive);

        result.ActiveEmployees = await _context.Set<Employee>()
            .CountAsync(x => !x.IsDeleted && x.IsActive);

        result.TopSendingBranches = await cargos
            .GroupBy(x => x.OriginBranch.BranchName)
            .Select(g => new ReportBranchRow
            {
                BranchName = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        result.TopDeliveringBranches = await cargos
            .Where(x => x.Status == CargoStatus.Delivered)
            .GroupBy(x => x.DestinationBranch.BranchName)
            .Select(g => new ReportBranchRow
            {
                BranchName = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        var dailyCounts = await cargos
            .Where(x =>
                x.CreatedDate >= sevenDaysAgo &&
                x.CreatedDate < today.AddDays(1))
            .GroupBy(x => x.CreatedDate.Date)
            .Select(g => new ReportDayRow
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        result.Last7Days = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var day = sevenDaysAgo.AddDays(i);

                return new ReportDayRow
                {
                    Date = day,
                    Count = dailyCounts
                        .FirstOrDefault(x => x.Date == day)?.Count ?? 0
                };
            })
            .ToList();

        var deliveredByEmployee = await _context.Set<Delivery>()
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.VerifiedAt.HasValue &&
                x.DeliveredByEmployeeId.HasValue)
            .GroupBy(x => x.DeliveredByEmployeeId!.Value)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var failedByEmployee = await _context.Set<DeliveryException>()
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.EmployeeId.HasValue)
            .GroupBy(x => x.EmployeeId!.Value)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var employeeIds = deliveredByEmployee
            .Select(x => x.EmployeeId)
            .Concat(failedByEmployee.Select(x => x.EmployeeId))
            .Distinct()
            .ToArray();

        var employees = await _context.Set<Employee>()
            .AsNoTracking()
            .Where(x => employeeIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.FullName
            })
            .ToListAsync();

        result.EmployeePerformance = employees
            .Select(x => new ReportEmployeeRow
            {
                EmployeeName = x.FullName,

                DeliveredCount = deliveredByEmployee
                    .FirstOrDefault(y => y.EmployeeId == x.Id)?.Count ?? 0,

                FailedCount = failedByEmployee
                    .FirstOrDefault(y => y.EmployeeId == x.Id)?.Count ?? 0
            })
            .OrderByDescending(x => x.DeliveredCount)
            .ThenByDescending(x => x.FailedCount)
            .Take(10)
            .ToList();

        return result;
    }
}
