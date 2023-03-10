using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Diagnostics;

namespace CSharpWars.Orleans.Grains;

public class ArenaState
{
    public bool Exists { get; set; }
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public IList<Guid>? BotIds { get; set; }
}

public class ArenaGrain : GrainBase<IArenaGrain>, IArenaGrain
{
    private readonly IGrainFactoryHelperWithStringKey<IPlayerGrain> _playerGrainFactory;
    private readonly IGrainFactoryHelperWithGuidKey<IBotGrain> _botGrainFactory;
    private readonly IGrainFactoryHelperWithStringKey<IProcessingGrain> _processingGrainFactory;

    private readonly IPersistentState<ArenaState> _state;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IArenaGrain> _logger;

    public ArenaGrain(
        IGrainFactoryHelperWithStringKey<IPlayerGrain> playerGrainFactory,
        IGrainFactoryHelperWithGuidKey<IBotGrain> botGrainFactory,
        IGrainFactoryHelperWithStringKey<IProcessingGrain> processingGrainFactory,
        [PersistentState("arena", "arenaStore")] IPersistentState<ArenaState> state,
        IConfiguration configuration, ILogger<IArenaGrain> logger) : base(logger)
    {
        _playerGrainFactory = playerGrainFactory;
        _botGrainFactory = botGrainFactory;
        _processingGrainFactory = processingGrainFactory;
        _state = state;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<BotDto>> GetAllActiveBots()
    {
        var activeBots = await GetBots(false);

        return activeBots;
    }

    public async Task<List<BotDto>> GetAllLiveBots()
    {
        return await GetBots(true);
    }

    private async Task<List<BotDto>> GetBots(bool onlyLive)
    {
        if (!_state.State.Exists)
        {
            _ = await GetArenaDetails();
        }

        var activeBots = new List<BotDto>();

        if (_state.State.BotIds != null)
        {
            var botStates = new List<Task<BotDto>>();

            for (int i = 0; i < _state.State.BotIds.Count; i++)
            {
                Guid botId = _state.State.BotIds[i];
                var botState = await _botGrainFactory.FromGrain(botId, g => g.GetState());

                if (!onlyLive || botState.Move != Move.Died)
                {
                    activeBots.Add(botState);
                }
            }
        }

        return activeBots;
    }

    public async Task<ArenaDto> GetArenaDetails()
    {
        _logger.AutoLogInformation($"Getting arena details for '{this.GetPrimaryKeyString()}'");

        if (!_state.State.Exists)
        {
            _logger.AutoLogInformation($"Creating new arena '{this.GetPrimaryKeyString()}'");

            _state.State.Name = this.GetPrimaryKeyString();
            _state.State.Width = _configuration.GetValue<int>("ARENA_WIDTH");
            _state.State.Height = _configuration.GetValue<int>("ARENA_HEIGHT");
            _state.State.BotIds = new List<Guid>();
            _state.State.Exists = true;
            await _state.WriteStateAsync();
        }

        return new ArenaDto
        {
            Name = _state.State.Name,
            Width = _state.State.Width,
            Height = _state.State.Height
        };
    }

    public async Task<BotDto> CreateBot(string playerName, BotToCreateDto bot)
    {
        if (!_state.State.Exists || _state.State.BotIds == null)
        {
            throw new ArgumentNullException();
        }

        var startTime = Stopwatch.GetTimestamp();

        var botId = Guid.NewGuid();

        var arenaDetails = await GetArenaDetails();
        var activeBots = await GetAllActiveBots();

        await _playerGrainFactory.FromGrain(playerName, g => g.ValidateBotDeploymentLimit());
        var createdBot = await _botGrainFactory.FromGrain(botId, g => g.CreateBot(bot, arenaDetails, activeBots));
        await _playerGrainFactory.FromGrain(playerName, g => g.BotCreated(botId));

        _state.State.BotIds.Add(botId);
        await _state.WriteStateAsync();

        _logger.LogInformation($"CreateBot: {Stopwatch.GetElapsedTime(startTime).TotalSeconds:F0}ms");

        return createdBot;
    }

    public async Task DeleteArena()
    {
        if (_state.State.Exists && _state.State.BotIds != null)
        {
            await _processingGrainFactory.FromGrain(_state.State.Name, g => g.Stop());

            await Task.Delay(2000);

            await DeleteBots(_state.State.BotIds.ToArray());

            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }

    public async Task DeleteBot(Guid botId)
    {
        if (_state.State.Exists && _state.State.BotIds != null)
        {
            _state.State.BotIds.Remove(botId);
            await _state.WriteStateAsync();
        }
    }

    public async Task DeleteBots(Guid[] botIds)
    {
        if (_state.State.Exists && _state.State.BotIds != null)
        {
            var deleteBotTasks = new List<Task>();

            foreach (var botId in botIds)
            {
                deleteBotTasks.Add(_botGrainFactory.FromGrain(botId, g => g.DeleteBot()));
                _state.State.BotIds.Remove(botId);
            }

            await Task.WhenAll(deleteBotTasks);
            await _state.WriteStateAsync();
        }
    }
}