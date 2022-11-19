namespace CSharpWars.WebApi.Contracts;

public class GetArenaRequest
{
    public string Name { get; set; }
}

public class GetArenaResponse
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}