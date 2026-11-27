
using CargoTrack.Entity.Enums;

namespace CargoTrack.DataAccess.Reports;

public class UserDashboardSnapshot
{
    public int TotalCargos { get; set; }
    public int IncomingCargos { get; set; }
    public int OutgoingCargos { get; set; }
    public int ActiveCargos { get; set; }
    public int DeliveredCargos { get; set; }
    public int ReturnedCargos { get; set; }

    public List<UserDashboardCargoSnapshot> RecentCargos
    { get; set; } = new();

    public List<UserDashboardMovementSnapshot> RecentMovements
    { get; set; } = new();
}

public class UserDashboardCargoSnapshot
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public string Direction { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
}

public class UserDashboardMovementSnapshot
{
    public Guid CargoId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public DateTime MovementDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
