using CSharpWars.Orleans.Contracts.Player;
using CSharpWars.Orleans.Grains.Helpers;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class PlayersState
{
    public bool Exists { get; set; }
    public IList<string>? PlayerNames { get; set; }
}

public interface IPlayersGrain : IGrainWithGuidKey
{
    Task<PlayerDto> Login(string username, string password);
    Task DeleteAllPlayers();
}

public class PlayersGrain : Grain, IPlayersGrain
{
    private readonly IPersistentState<PlayersState> _state;
    private readonly IGrainFactoryHelperWithStringKey<IPlayerGrain> _playerGrainFactoryHelper;

    public PlayersGrain(
        [PersistentState("players", "playersStore")] IPersistentState<PlayersState> state,
        IGrainFactoryHelperWithStringKey<IPlayerGrain> playerGrainFactoryHelper)
    {
        _state = state;
        _playerGrainFactoryHelper = playerGrainFactoryHelper;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        if (!_state.State.Exists)
        {
            _state.State.PlayerNames = new List<string>();
            _state.State.Exists = true;

            await _state.WriteStateAsync();
        }

        var player = await _playerGrainFactoryHelper.FromGrain(
            username, g => g.Login(username, password));

        if (_state.State.PlayerNames != null && !_state.State.PlayerNames.Contains(username))
        {
            _state.State.PlayerNames.Add(username);
        }

        return player;
    }

    public async Task DeleteAllPlayers()
    {
        if (_state.State.Exists && _state.State.PlayerNames != null)
        {
            foreach (var playerName in _state.State.PlayerNames)
            {
                await _playerGrainFactoryHelper.FromGrain(playerName, g => g.DeletePlayer());
            }

            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }
}