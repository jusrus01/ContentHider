using ContentHider.Core.Dtos.Auth;

namespace ContentHider.Core.Services;

public interface IAuthService
{
    Task<TokenDto> GenerateTokenAsync(RequestRefreshTokenDto model);
    Task<TokenDto> GenerateTokenAsync(RequestTokenDto model);
    Task<ResponseRegisterDto> RegisterAsync(RequestRegisterDto model);
}