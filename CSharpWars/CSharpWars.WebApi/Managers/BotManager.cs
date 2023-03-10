using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;
using System.Diagnostics;

namespace CSharpWars.WebApi.Managers;

public interface IBotManager
{
    Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request);

    Task<CreateBotResponse> CreateBot(CreateBotRequest request);
}

public class BotManager : IBotManager
{
    private readonly IGrainFactoryHelperWithStringKey<IArenaGrain> _arenaGrainClient;
    private readonly IGrainFactoryHelperWithStringKey<IProcessingGrain> _processingGrainClient;
    private readonly IMapper _mapper;
    private readonly ILogger<BotManager> _logger;

    public BotManager(
        IGrainFactoryHelperWithStringKey<IArenaGrain> arenaGrainClient,
        IGrainFactoryHelperWithStringKey<IProcessingGrain> processingGrainClient,
        IMapper mapper, ILogger<BotManager> logger)
    {
        _arenaGrainClient = arenaGrainClient;
        _processingGrainClient = processingGrainClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request)
    {
        var startTime = Stopwatch.GetTimestamp();

        var bots = await _arenaGrainClient.FromGrain(request.ArenaName, g => g.GetAllActiveBots());
        await _processingGrainClient.FromGrain(request.ArenaName, g => g.Ping());

        _logger.LogInformation($"GetAllActiveBots: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");

        return _mapper.Map<GetAllActiveBotsResponse>(bots);
    }

    public async Task<CreateBotResponse> CreateBot(CreateBotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerName))
        {
            throw new ArgumentNullException(nameof(request.PlayerName));
        }

        var botToCreate = _mapper.Map<BotToCreateDto>(request);

        var bot = await _arenaGrainClient.FromGrain(request.ArenaName, g => g.CreateBot(request.PlayerName, botToCreate));
        return _mapper.Map<CreateBotResponse>(bot);
    }
}