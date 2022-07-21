using CSharpWars.Orleans.Contracts.Arena;
using CSharpWars.Orleans.Contracts.Bot;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class ArenaState
{
    public bool Exists { get; set; }
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Guid> BotIds { get; set; }
}

public interface IArenaGrain : IGrainWithStringKey
{
    Task<List<BotDto>> GetAllActiveBots();

    Task<ArenaDto> GetArenaDetails();

    Task<BotDto> CreateBot(string playerName, BotToCreateDto bot);
    Task DeleteArena();
}

public class ArenaGrain : Grain, IArenaGrain
{
    private readonly IPersistentState<ArenaState> _state;
    private readonly IConfiguration _configuration;

    private IDisposable? _timer;

    public ArenaGrain(
        [PersistentState("arena", "arenaStore")] IPersistentState<ArenaState> state,
        IConfiguration configuration)
    {
        _state = state;
        _configuration = configuration;
    }

    public override Task OnActivateAsync()
    {
        _timer = RegisterTimer(OnTimer, _state, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));

        return base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
        if (_timer != null)
        {
            _timer.Dispose();
        }

        return base.OnDeactivateAsync();
    }

    private async Task OnTimer(object state)
    {
        if (_state.State.Exists)
        {
            var tasks = new List<Task>();

            foreach (var botId in _state.State.BotIds)
            {
                var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
                tasks.Add(botGrain.Process());
            }

            await Task.WhenAll(tasks);
        }
    }

    public async Task<List<BotDto>> GetAllActiveBots()
    {
        if (!_state.State.Exists)
        {
            _ = await GetArenaDetails();
        }

        var activeBots = new List<BotDto>();

        foreach (var botId in _state.State.BotIds)
        {
            var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
            activeBots.Add(await botGrain.GetState());
        }

        return activeBots;
    }

    public async Task<ArenaDto> GetArenaDetails()
    {
        if (!_state.State.Exists)
        {
            _state.State.Name = this.GetPrimaryKeyString();
            _state.State.Width = _configuration.GetValue<int>("ARENA_WIDTH");
            _state.State.Height = _configuration.GetValue<int>("ARENA_HEIGHT");
            _state.State.BotIds = new List<Guid>();
            _state.State.Exists = true;
            await _state.WriteStateAsync();
        }

        return new ArenaDto(_state.State.Name, _state.State.Width, _state.State.Height);
    }

    public async Task<BotDto> CreateBot(string playerName, BotToCreateDto bot)
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentNullException();
        }

        var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerName);
        await playerGrain.ValidateBotDeploymentLimit();

        var botId = Guid.NewGuid();

        var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
        var createdBot = await botGrain.CreateBot(bot);

        _state.State.BotIds.Add(botId);
        await _state.WriteStateAsync();

        return createdBot;
    }

    public async Task DeleteArena()
    {
        if (_state.State.Exists)
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }

            await Task.Delay(2000);

            foreach (var botId in _state.State.BotIds)
            {
                var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
                await botGrain.DeleteBot();
            }

            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }
}