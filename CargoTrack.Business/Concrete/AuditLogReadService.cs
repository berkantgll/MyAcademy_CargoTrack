
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.AuditDtos;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class AuditLogReadService : IAuditLogReadService
{
    private readonly IAuditLogReadRepository _repository;

    public AuditLogReadService(
        IAuditLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditLogPageDto> GetPageAsync(
        int page, int pageSize)
    {
        page = Math.Clamp(page, 1, 10000);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository
            .GetPageAsync(page, pageSize);

        return new AuditLogPageDto
        {
            Items = result.Items.Adapt<List<AuditLogItemDto>>(),
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }
}
