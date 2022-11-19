namespace CSharpWars.WebApi.Contracts;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Username { get; set; }
    public string Token { get; set; }
}