namespace CSharpWars.WebApi.Contracts;

public record GetAllActiveBotsRequest(string ArenaName);

public record GetAllActiveBotsResponse(List<Bot> Bots);

public record Bot(Guid BotId, string BotName);

public record CreateBotRequest(
    string PlayerName,
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