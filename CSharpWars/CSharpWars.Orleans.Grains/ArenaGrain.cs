using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public interface IArenaGrain : IGrainWithGuidKey
{
    Task<string> GetMessage();
    Task IncreaseCounter();
}

public class ArenaState
{
    public int Counter { get; set; }
}

public class ArenaGrain : Grain, IArenaGrain
{
    private readonly IPersistentState<ArenaState> _state;
    
    public ArenaGrain(
        [PersistentState("arena", "arenaStore")] IPersistentState<ArenaState> state)
    {
        _state = state;
    }

    public Task<string> GetMessage()
    {
        return Task.FromResult($"[{_state.State.Counter}] - {IdentityString}");
    }

    public async Task IncreaseCounter()
    {
        _state.State.Counter++;
        await _state.WriteStateAsync();
    }
}