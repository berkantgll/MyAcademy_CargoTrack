
using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.DashboardDtos;

public class ManagerDashboardDto
{
    public string BranchName { get; set; } = string.Empty;

    public int TotalCargos { get; set; }
    public int TodayIncoming { get; set; }
    public int TodayOutgoing { get; set; }
    public int OutForDelivery { get; set; }
    public int DeliveredToday { get; set; }
    public int DelayedCargos { get; set; }
    public int ProblematicCargos { get; set; }

    public List<ManagerMovementDto> RecentMovements
    { get; set; } = new();
}

public class ManagerMovementDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public DateTime MovementDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
