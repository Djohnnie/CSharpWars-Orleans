namespace CSharpWars.WebApi.Contracts;

public class GetArenaRequest
{
    public string Name { get; set; } = string.Empty;
}

public class GetArenaResponse
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}