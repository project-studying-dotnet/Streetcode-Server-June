using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.BLL.Services.CookieService.Realizations;
using Streetcode.BLL.Services.Tokens;
using System.Linq;
using System.Runtime.Serialization;
using Xunit;

namespace Streetcode.XUnitTest.Services.CookieServiceTests
{
    public class CookieServiceTests
    {
        [Theory]
        [InlineData("test1", "someValue1")]
        [InlineData("test2", "someValue2")]
        [InlineData("test3", "someValue3")]
        public async Task Success_When_AddingInfoToCookies(string key, string value)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            var cookieService = CreateCookieService();

            var expires = DateTimeOffset.UtcNow.AddMinutes(17);

            // Act
            await cookieService!.
                AppendCookiesToResponseAsync(httpContext.Response, (key, value, new CookieOptions() 
                { 
                    Expires = expires,                    
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                }));
            
            // Assert
            Assert.True(httpContext.Response.Headers.Values.Count > 0);
        }
       
        public static ICookieService? CreateCookieService()
        {
            IServiceCollection col = new ServiceCollection();

            col.AddSingleton<ICookieService, CookieService>();

            return col.BuildServiceProvider().GetService<ICookieService>() ?? null;
        }
    }
}
