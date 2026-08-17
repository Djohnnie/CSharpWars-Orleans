namespace CSharpWars.WebApi.Contracts;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}