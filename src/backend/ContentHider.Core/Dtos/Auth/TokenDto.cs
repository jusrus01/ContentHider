namespace ContentHider.Core.Dtos.Auth;

public class TokenDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}