namespace CSharpWars.Orleans.Contracts.Bot;

public record BotToCreateDto(
    string PlayerName,
    string BotName,
    string ArenaName,
    int MaximumHealth,
    int MaximumStamina,
    string Script);