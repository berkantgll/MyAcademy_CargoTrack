
using CargoTrack.Entity.Enums;

namespace CargoTrack.DataAccess.Reports;

public class AdminReportSnapshot
{
    public int TotalCargos { get; set; }
    public int DeliveredCargos { get; set; }
    public int ReturnedCargos { get; set; }
    public int DelayedCargos { get; set; }
    public int OnTimeDeliveries { get; set; }
    public int ActiveBranches { get; set; }
    public int ActiveEmployees { get; set; }

    public double AverageDeliveryHours { get; set; }

    public List<ReportStatusRow> StatusDistribution { get; set; } = new();
    public List<ReportBranchRow> TopSendingBranches { get; set; } = new();
    public List<ReportBranchRow> TopDeliveringBranches { get; set; } = new();
    public List<ReportDayRow> Last7Days { get; set; } = new();
    public List<ReportEmployeeRow> EmployeePerformance { get; set; } = new();
}

public class ReportStatusRow
{
    public CargoStatus Status { get; set; }
    public int Count { get; set; }
}

public class ReportBranchRow
{
    public string BranchName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportDayRow
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class ReportEmployeeRow
{
    public string EmployeeName { get; set; } = string.Empty;
    public int DeliveredCount { get; set; }
    public int FailedCount { get; set; }
}
