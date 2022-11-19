using CSharpWars.Enums;
using MoveEnum = CSharpWars.Enums.Move;

namespace CSharpWars.WebApi.Contracts;

public class GetAllActiveBotsRequest
{
    public string ArenaName { get; set; }
}

public class GetAllActiveBotsResponse
{
    public List<Bot> Bots { get; set; }
}

public class Bot
{
    public Guid BotId { get; init; }
    public string BotName { get; init; }
    public string PlayerName { get; init; }
    public int CurrentHealth { get; init; }
    public int MaximumHealth { get; init; }
    public int CurrentStamina { get; init; }
    public int MaximumStamina { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int FromX { get; init; }
    public int FromY { get; init; }
    public int LastAttackX { get; init; }
    public int LastAttackY { get; init; }
    public Orientation Orientation { get; init; }
    public MoveEnum Move { get; init; }
}

public class CreateBotRequest
{
    public string? PlayerName { get; set; }
    public string BotName { get; set; }
    public string ArenaName { get; set; }
    public int MaximumHealth { get; set; }
    public int MaximumStamina { get; set; }
    public string Script { get; set; }
}

public class CreateBotResponse
{
    public Guid BotId { get; set; }
    public string BotName { get; set; }
    public int MaximumHealth { get; set; }
    public int MaximumStamina { get; set; }
}