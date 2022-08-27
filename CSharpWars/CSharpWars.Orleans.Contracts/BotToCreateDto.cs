namespace CSharpWars.Orleans.Contracts;

public record BotToCreateDto(
    string PlayerName,
    string BotName,
    string ArenaName,
    int MaximumHealth,
    int MaximumStamina,
    string Script);