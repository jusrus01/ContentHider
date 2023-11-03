namespace ContentHider.Core.Dtos.Auth;

public class JwtOptions
{
    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public double TokenValidityInMinutes { get; init; }
    public double RefreshTokenValidityInDays { get; init; }
}