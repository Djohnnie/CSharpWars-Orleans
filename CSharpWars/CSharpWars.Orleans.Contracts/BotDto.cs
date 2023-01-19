using CSharpWars.Enums;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class BotDto
{
    [Id(0)]
    public Guid BotId { get; set; }
    [Id(1)]
    public string BotName { get; set; }
    [Id(2)]
    public string PlayerName { get; set; }
    [Id(3)]
    public int CurrentHealth { get; set; }
    [Id(4)]
    public int MaximumHealth { get; set; }
    [Id(5)]
    public int CurrentStamina { get; set; }
    [Id(6)]
    public int MaximumStamina { get; set; }
    [Id(7)]
    public int X { get; set; }
    [Id(8)]
    public int Y { get; set; }
    [Id(9)]
    public int FromX { get; set; }
    [Id(10)]
    public int FromY { get; set; }
    [Id(11)]
    public int LastAttackX { get; set; }
    [Id(12)]
    public int LastAttackY { get; set; }
    [Id(13)]
    public Orientation Orientation { get; set; }
    [Id(14)]
    public Move Move { get; set; }
    [Id(15)]
    public DateTime? TimeOfDeath { get; set; }
    [Id(16)]
    public string Memory { get; set; }
}