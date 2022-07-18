namespace CSharpWars.WebApi.Contracts;

public record GetArenaRequest(string Name);
public record GetArenaResponse(string Name, int Width, int Height);