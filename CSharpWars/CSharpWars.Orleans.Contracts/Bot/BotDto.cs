using CSharpWars.Enums;

namespace CSharpWars.Orleans.Contracts.Bot;

public class BotDto
{
    public Guid BotId { get; set; }
    public string BotName { get; set; }
    public string PlayerName { get; set; }
    public int CurrentHealth { get; set; }
    public int MaximumHealth { get; set; }
    public int CurrentStamina { get; set; }
    public int MaximumStamina { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int FromX { get; set; }
    public int FromY { get; set; }
    public int LastAttackX { get; set; }
    public int LastAttackY { get; set; }
    public Orientation Orientation { get; set; }
    public Move Move { get; set; }
    public DateTime TimeOfDeath { get; set; }
    public string Memory { get; set; }
}