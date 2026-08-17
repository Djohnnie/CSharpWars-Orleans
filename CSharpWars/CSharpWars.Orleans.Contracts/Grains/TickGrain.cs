namespace CSharpWars.Orleans.Contracts.Grains;

public interface ITickGrain : IGrainWithStringKey
{
    /// <summary>
    /// Atomically persists bot state updates for a single game tick.
    /// Either all updates succeed (with the tick applied) or all fail (no partial state).
    /// Returns the tick number that was applied.
    /// </summary>
    Task<long> PersistTickAsync(List<BotDto> botsToUpdate);

    /// <summary>
    /// Gets the current tick number for idempotency tracking.
    /// </summary>
    Task<long> GetCurrentTickAsync();
}
