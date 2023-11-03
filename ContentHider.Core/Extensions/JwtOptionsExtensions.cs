using System.Text;
using ContentHider.Core.Dtos.Auth;
using Microsoft.IdentityModel.Tokens;

namespace ContentHider.Core.Extensions;

public static class JwtOptionsExtensions
{
    public static SymmetricSecurityKey GetIssuerSigningKey(this JwtOptions options)
    {
        return GetIssuerSigningKey(options.Key);
    }

    public static SymmetricSecurityKey GetIssuerSigningKey(this string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}