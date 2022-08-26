using CSharpWars.Enums;

namespace CSharpWars.WebApi.Contracts;

public record GetAllActiveBotsRequest(string ArenaName);

public record GetAllActiveBotsResponse(List<Bot> Bots);

public record Bot
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
    public Move Move { get; init; }
}

public record CreateBotRequest(
    string? PlayerName,
    string BotName,
    string ArenaName,
    int MaximumHealth,
    int MaximumStamina,
    string Script);

public record CreateBotResponse(
    Guid BotId,
    string BotName,
    int MaximumHealth,
    int MaximumStamina);