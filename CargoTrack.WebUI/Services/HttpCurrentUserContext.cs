
using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Http;

namespace CargoTrack.WebUI.Services;

public class HttpCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserContext(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserName
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.Identity.Name ?? "AuthenticatedUser";
            }

            return "System";
        }
    }
}
