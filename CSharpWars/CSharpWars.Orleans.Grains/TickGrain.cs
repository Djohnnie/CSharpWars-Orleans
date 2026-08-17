using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class TickState
{
    /// <summary>
    /// Monotonically increasing tick counter for idempotency.
    /// Allows recovery from partial failures by tracking which tick was last applied.
    /// </summary>
    public long CurrentTick { get; set; }
}

public class TickGrain : GrainBase<ITickGrain>, ITickGrain
{
    private readonly IGrainFactoryHelperWithGuidKey<IBotGrain> _botGrainFactory;
    private readonly ILogger<ITickGrain> _logger;
    private readonly IPersistentState<TickState> _state;

    public TickGrain(
        IGrainFactoryHelperWithGuidKey<IBotGrain> botGrainFactory,
        ILogger<ITickGrain> logger,
        [PersistentState("tick", "tickStore")] IPersistentState<TickState> state) : base(logger)
    {
        _botGrainFactory = botGrainFactory;
        _logger = logger;
        _state = state;
    }

    public async Task<long> PersistTickAsync(List<BotDto> botsToUpdate)
    {
        // Increment tick counter before applying updates
        var newTick = _state.State.CurrentTick + 1;

        try
        {
            // Collect all bot update tasks within a transactional context
            var updateTasks = new List<Task>();
            foreach (var bot in botsToUpdate)
            {
                // Pass tick number for idempotent updates
                updateTasks.Add(_botGrainFactory.FromGrain(bot.BotId, g => g.UpdateStateTransactional(bot, newTick)));
            }

            // Execute all updates atomically - all succeed or all fail
            await Task.WhenAll(updateTasks);

            // Only persist tick counter if all bot updates succeeded
            _state.State.CurrentTick = newTick;
            await _state.WriteStateAsync();

            _logger.LogInformation($"Successfully persisted tick {newTick} for arena '{this.GetPrimaryKeyString()}' with {botsToUpdate.Count} bot updates");

            return newTick;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to persist tick {newTick} for arena '{this.GetPrimaryKeyString()}'. Tick counter remains at {_state.State.CurrentTick}. This tick will be automatically retried by the processor.");
            throw;
        }
    }

    public Task<long> GetCurrentTickAsync()
    {
        return Task.FromResult(_state.State.CurrentTick);
    }
}
