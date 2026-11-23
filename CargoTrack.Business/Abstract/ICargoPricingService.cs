
using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Abstract;

public interface ICargoPricingService
{
    Task<decimal> CalculatePriceAsync(Cargo cargo);
}
