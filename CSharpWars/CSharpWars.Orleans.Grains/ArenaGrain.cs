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

    Task<BotDto> CreateBot(BotToCreateDto bot);
}

public class ArenaGrain : Grain, IArenaGrain
{
    private readonly IPersistentState<ArenaState> _state;
    private readonly IConfiguration _configuration;

    public ArenaGrain(
        [PersistentState("arena", "arenaStore")] IPersistentState<ArenaState> state,
        IConfiguration configuration)
    {
        _state = state;
        _configuration = configuration;
    }

    public async Task<List<BotDto>> GetAllActiveBots()
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentNullException();
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

    public async Task<BotDto> CreateBot(BotToCreateDto bot)
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentNullException();
        }

        var botId = Guid.NewGuid();

        var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
        var createdBot = await botGrain.CreateBot(bot);

        _state.State.BotIds.Add(botId);

        return createdBot;
    }
}