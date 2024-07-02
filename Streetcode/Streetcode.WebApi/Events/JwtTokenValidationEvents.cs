using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.WebApi.Events;

public class JwtTokenValidationEvents : JwtBearerEvents
{
    private readonly ILogger<JwtTokenValidationEvents> _logger;
    private readonly ICacheService _cacheService;
    public JwtTokenValidationEvents(ILogger<JwtTokenValidationEvents> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }


    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var accessToken = (context.SecurityToken as JwtSecurityToken)?.RawData;
        if (await _cacheService.IsBlacklistedTokenAsync(accessToken!))
        {  
            _logger.LogWarning("Token is blacklisted: {AccessToken}", accessToken);
            context.Fail("Token is blacklisted");
            return;
        }
        
        _logger.LogInformation("Token validated.");
        await base.TokenValidated(context);
    }
}
