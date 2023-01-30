namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class BotToCreateDto
{
    [Id(0)]
    public string PlayerName { get; set; }
    [Id(1)]
    public string BotName { get; set; }
    [Id(2)]
    public string ArenaName { get; set; }
    [Id(3)]
    public int MaximumHealth { get; set; }
    [Id(4)]
    public int MaximumStamina { get; set; }
    [Id(5)]
    public string Script { get; set; }
}