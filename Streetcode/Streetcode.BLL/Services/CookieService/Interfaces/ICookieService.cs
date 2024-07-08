using Microsoft.AspNetCore.Http;

namespace Streetcode.BLL.Services.CookieService.Interfaces
{
    public interface ICookieService
    {
        Task ClearRequestCookiesAsync(HttpContext httpContext);

        Task AppendCookiesToResponseAsync(HttpResponse httpResponse, params (string key, string value, CookieOptions options)[] values);
    }
}
