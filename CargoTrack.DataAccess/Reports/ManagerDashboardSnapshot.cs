
using CargoTrack.Entity.Enums;

namespace CargoTrack.DataAccess.Reports;

public class ManagerDashboardSnapshot
{
    public string BranchName { get; set; } = string.Empty;

    public int TotalCargos { get; set; }
    public int TodayIncoming { get; set; }
    public int TodayOutgoing { get; set; }
    public int OutForDelivery { get; set; }
    public int DeliveredToday { get; set; }
    public int DelayedCargos { get; set; }
    public int ProblematicCargos { get; set; }

    public List<ManagerMovementSnapshot> RecentMovements
    { get; set; } = new();
}

public class ManagerMovementSnapshot
{
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public DateTime MovementDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
