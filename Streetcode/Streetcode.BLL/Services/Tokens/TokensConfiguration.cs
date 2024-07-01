namespace Streetcode.BLL.Services.Tokens;

public class TokensConfiguration
{
    public double AccessTokenExpirationMinutes { get; set; }
    public double RefreshTokenExpirationDays { get; set; }
    public string? SecretKey { get; set; } = string.Empty;
    public string? Issuer { get; set; } = string.Empty;
    public string? Audience { get; set; } = string.Empty;
}