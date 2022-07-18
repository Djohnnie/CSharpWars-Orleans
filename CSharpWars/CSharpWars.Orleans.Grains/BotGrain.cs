using CSharpWars.Orleans.Contracts.Bot;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class BotState
{
    public bool Exists { get; set; }
    public string BotName { get; set; }
    public int MaximumHealth { get; set; }
    public int MaximumStamina { get; set; }
    public string Script { get; set; }
}

public interface IBotGrain : IGrainWithGuidKey
{
    Task<BotDto> GetState();

    Task<BotDto> CreateBot(BotToCreateDto bot);
}

public class BotGrain : Grain, IBotGrain
{
    private readonly IPersistentState<BotState> _state;

    public BotGrain(
        [PersistentState("bot", "botStore")] IPersistentState<BotState> state)
    {
        _state = state;
    }

    public Task<BotDto> GetState()
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentNullException();
        }

        return Task.FromResult(new BotDto(
            this.GetPrimaryKey(),
            _state.State.BotName,
            _state.State.MaximumHealth,
            _state.State.MaximumStamina));
    }

    public async Task<BotDto> CreateBot(BotToCreateDto bot)
    {
        if (_state.State.Exists)
        {
            throw new ArgumentNullException();
        }

        _state.State.BotName = bot.BotName;
        _state.State.MaximumHealth = bot.MaximumHealth;
        _state.State.MaximumStamina = bot.MaximumStamina;
        _state.State.Script = bot.Script;
        _state.State.Exists = true;

        return await GetState();
    }
}