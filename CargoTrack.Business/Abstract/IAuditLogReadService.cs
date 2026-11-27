
using CargoTrack.DTO.AuditDtos;

namespace CargoTrack.Business.Abstract;

public interface IAuditLogReadService
{
    Task<AuditLogPageDto> GetPageAsync(int page, int pageSize);
}
