namespace CSharpWars.Orleans.Contracts.Bot;

public record BotDto(Guid BotId, string BotName, int MaximumHealth, int MaximumStamina);