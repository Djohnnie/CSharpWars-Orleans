using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Contracts.Player;
using CSharpWars.Orleans.Grains.Base;
using CSharpWars.Orleans.Grains.Helpers;
using Microsoft.Extensions.Logging;
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

public class PlayersGrain : GrainBase<IPlayersGrain>, IPlayersGrain
{
    private readonly IPersistentState<PlayersState> _state;
    private readonly IGrainFactoryHelperWithStringKey<IPlayerGrain> _playerGrainFactoryHelper;
    private readonly ILogger<IPlayersGrain> _logger;

    public PlayersGrain(
        [PersistentState("players", "playersStore")] IPersistentState<PlayersState> state,
        IGrainFactoryHelperWithStringKey<IPlayerGrain> playerGrainFactoryHelper,
        ILogger<IPlayersGrain> logger) : base(logger)
    {
        _state = state;
        _playerGrainFactoryHelper = playerGrainFactoryHelper;
        _logger = logger;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        if (!_state.State.Exists)
        {
            _logger.AutoLogInformation($"Create state for {nameof(PlayersGrain)}");

            _state.State.PlayerNames = new List<string>();
            _state.State.Exists = true;

            await _state.WriteStateAsync();
        }

        var player = await _playerGrainFactoryHelper.FromGrain(
            username, g => g.Login(username, password));

        if (_state.State.PlayerNames != null && !_state.State.PlayerNames.Contains(username))
        {
            _logger.AutoLogInformation($"Add '{username}' to {nameof(PlayersGrain)} state");

            _state.State.PlayerNames.Add(username);
            await _state.WriteStateAsync();
        }

        return player;
    }

    public async Task DeleteAllPlayers()
    {
        if (_state.State.Exists && _state.State.PlayerNames != null)
        {
            _logger.AutoLogInformation("All players will be deleted");

            foreach (var playerName in _state.State.PlayerNames)
            {
                await _playerGrainFactoryHelper.FromGrain(playerName, g => g.DeletePlayer());
            }

            await _state.ClearStateAsync();
        }

        _logger.AutoLogInformation($"{nameof(PlayersGrain)} will be deactivated...");
        DeactivateOnIdle();
    }
}