using CSharpWars.Orleans.Contracts.Player;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class PlayersState
{
    public bool Exists { get; set; }
    public List<string> PlayerNames { get; set; }
}

public interface IPlayersGrain : IGrainWithStringKey
{
    Task<PlayerDto> Login(string username, string password);
    Task DeleteAllPlayers();
}

public class PlayersGrain : Grain, IPlayersGrain
{
    private readonly IPersistentState<PlayersState> _state;

    public PlayersGrain(
        [PersistentState("players", "playersStore")] IPersistentState<PlayersState> state)
    {
        _state = state;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        if (!_state.State.Exists)
        {
            _state.State.PlayerNames = new();
            _state.State.Exists = true;

            await _state.WriteStateAsync();
        }

        var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(username);
        var player = await playerGrain.Login(username, password);

        if (!_state.State.PlayerNames.Contains(username))
        {
            _state.State.PlayerNames.Add(username);
        }

        return player;
    }

    public async Task DeleteAllPlayers()
    {
        if (_state.State.Exists)
        {
            foreach (var playerName in _state.State.PlayerNames)
            {
                var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerName);
                await playerGrain.DeletePlayer();
            }

            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }
}