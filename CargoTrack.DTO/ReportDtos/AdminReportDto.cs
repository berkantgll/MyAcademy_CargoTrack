
using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.ReportDtos;

public class AdminReportDto
{
    public int TotalCargos { get; set; }
    public int DeliveredCargos { get; set; }
    public int ReturnedCargos { get; set; }
    public int DelayedCargos { get; set; }
    public int OnTimeDeliveries { get; set; }
    public int ActiveBranches { get; set; }
    public int ActiveEmployees { get; set; }

    public double AverageDeliveryHours { get; set; }

    public int ActiveCargos =>
        Math.Max(0, TotalCargos - DeliveredCargos - ReturnedCargos);

    public double DeliveryRate => Percent(DeliveredCargos, TotalCargos);
    public double OnTimeRate => Percent(OnTimeDeliveries, DeliveredCargos);
    public double ReturnRate => Percent(ReturnedCargos, TotalCargos);

    public List<ReportStatusDto> StatusDistribution { get; set; } = new();
    public List<ReportBranchDto> TopSendingBranches { get; set; } = new();
    public List<ReportBranchDto> TopDeliveringBranches { get; set; } = new();
    public List<ReportDayDto> Last7Days { get; set; } = new();
    public List<ReportEmployeeDto> EmployeePerformance { get; set; } = new();

    private static double Percent(int count, int total)
    {
        return total == 0
            ? 0
            : Math.Round(100.0 * count / total, 1);
    }
}

public class ReportStatusDto
{
    public CargoStatus Status { get; set; }
    public int Count { get; set; }
}

public class ReportBranchDto
{
    public string BranchName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportDayDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class ReportEmployeeDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public int DeliveredCount { get; set; }
    public int FailedCount { get; set; }
}
