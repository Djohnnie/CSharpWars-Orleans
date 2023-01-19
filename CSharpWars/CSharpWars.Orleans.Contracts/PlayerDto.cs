namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class PlayerDto
{
    [Id(0)]
    public string Username { get; set; }
    [Id(1)]
    public string Token { get; set; }
}