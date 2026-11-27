
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Models;

public class AdminCargoSearchVm
{
    public string? Search { get; set; }
    public CargoStatus? Status { get; set; }
    public Guid? BranchId { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);

    public List<CargoListDto> Items { get; set; } = new();

    public List<SelectListItem> Branches { get; set; } = new();
}
