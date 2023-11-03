using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using ContentHider.Core;
using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Auth;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Extensions;
using ContentHider.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ContentHider.Domain;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwt;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<UserDao> _userManager;

    public AuthService(
        UserManager<UserDao> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtOptions> jwt)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
    }

    public async Task<TokenDto> GenerateTokenAsync(RequestRefreshTokenDto model)
    {
        var accessToken = model.Token;
        var refreshToken = model.RefreshToken;

        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            throw new HttpException(null, "Invalid access token or refresh token", HttpStatusCode.BadRequest);
        }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var id = principal.Claims.Single(claim => claim.Type == "uid").Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        var user = await _userManager.FindByIdAsync(id);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new HttpException(null, "Invalid access token or refresh token", HttpStatusCode.BadRequest);
        }

        if (user.RefreshToken != refreshToken)
        {
            throw new HttpException(null, "Invalid access token or refresh token", HttpStatusCode.BadRequest);
        }

        return await CreateTokenResponseAndLoginAsync(user);
    }

    public async Task<TokenDto> GenerateTokenAsync(RequestTokenDto model)
    {
        if (model == null)
        {
            throw new ArgumentNullException();
        }

        var tokenResponse = new TokenDto();

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            throw new HttpException(null, "No Accounts Registered with {model.Email}.", HttpStatusCode.BadRequest);
        }

        try
        {
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return await CreateTokenResponseAndLoginAsync(user);
            }
        }
        catch (ArgumentNullException)
        {
            // logger is null
        }

        throw new HttpException(null, $"Incorrect Credentials for user {user.Email}.", HttpStatusCode.BadRequest);
    }

    public async Task<ResponseRegisterDto> RegisterAsync(RequestRegisterDto model)
    {
        if (model == null)
        {
            throw new ArgumentNullException();
        }

        var user = new UserDao
        {
            UserName = model.Email,
            Email = model.Email
        };

        var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
        var userWithSameUsername = await _userManager.FindByNameAsync(model.Email);

        if (userWithSameEmail == null && userWithSameUsername == null)
        {
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Roles.User);
            }

            return new ResponseRegisterDto
            {
                AccountCreated = true,
                Message = $"User Registered with username {user.UserName}"
            };
        }

        return new ResponseRegisterDto
        {
            AccountCreated = false,
            Message = $"Email {user.Email} or username {user.UserName} already registered."
        };
    }

    private async Task<TokenDto> CreateTokenResponseAndLoginAsync(UserDao user)
    {
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        var expiration = DateTime.UtcNow.AddDays(_jwt.RefreshTokenValidityInDays);
        var refreshToken = CreateRefreshToken(expiration);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expiration;
        await _userManager.UpdateAsync(user);

        return new TokenDto
        {
            Token = await CreateJwtTokenAsStringAsync(user),
            Email = user.Email,
            UserName = user.UserName,
            Roles = rolesList.ToList(),
            RefreshToken = refreshToken
        };
    }

    private async Task<string> CreateJwtTokenAsStringAsync(UserDao user)
    {
        var token = await CreateJwtTokenAsync(user);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken(DateTime expiration)
    {
        var token = CreateJwtSecurityToken(expiration);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<JwtSecurityToken> CreateJwtTokenAsync(UserDao user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (var i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("role", roles[i]));
        }

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

        var jwtSecurityToken = CreateJwtSecurityToken(DateTime.UtcNow.AddMinutes(_jwt.TokenValidityInMinutes), claims);

        return jwtSecurityToken;
    }

    private JwtSecurityToken CreateJwtSecurityToken(DateTime expiration, IEnumerable<Claim>? claims = null)
    {
        var symmetricSecurityKey = _jwt.GetIssuerSigningKey();
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            expires: expiration,
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = _jwt.Audience,
            ValidIssuer = _jwt.Issuer,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwt.GetIssuerSigningKey(),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}