
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Models;

public class AdminCargoCreateVm
{
    public CreateCargoDto Cargo { get; set; } = new();

    public List<SelectListItem> Customers { get; set; } = new();
    public List<SelectListItem> Branches { get; set; } = new();
    public List<SelectListItem> Addresses { get; set; } = new();

    public List<ExtraServiceType> SelectedExtraServices { get; set; } = new();
}
