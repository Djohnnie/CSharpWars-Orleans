namespace CSharpWars.Orleans.Contracts.Grains;

public interface IBotGrain : IGrainWithGuidKey
{
    Task<BotDto> GetState();

    Task<BotDto> CreateBot(BotToCreateDto bot, ArenaDto arena, List<BotDto> activeBots);

    Task DeleteBot();

    Task UpdateState(BotDto bot);

    /// <summary>
    /// Atomically updates bot state within an Orleans transaction.
    /// Must only be called from within a transactional context.
    /// Idempotent: only applies state if tickNumber is newer than the last applied tick.
    /// </summary>
    Task UpdateStateTransactional(BotDto bot, long tickNumber);
}