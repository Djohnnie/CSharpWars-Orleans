namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class ArenaDto
{
    [Id(0)]
    public string Name { get; set; }
    [Id(1)]
    public int Width { get; set; }
    [Id(2)]
    public int Height { get; set; }
}