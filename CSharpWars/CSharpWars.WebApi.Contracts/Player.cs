namespace CSharpWars.WebApi.Contracts;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Username, string Token);