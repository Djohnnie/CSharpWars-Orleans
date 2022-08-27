using AutoMapper;
using CSharpWars.Orleans.Contracts.Bot;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Helpers;

namespace CSharpWars.WebApi.Managers;

public interface IBotManager
{
    Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request);

    Task<CreateBotResponse> CreateBot(CreateBotRequest request);
}

public class BotManager : IBotManager
{
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _arenaGrainClient;
    private readonly IClusterClientHelperWithStringKey<IProcessingGrain> _processingGrainClient;
    private readonly IMapper _mapper;

    public BotManager(
        IClusterClientHelperWithStringKey<IArenaGrain> arenaGrainClient,
        IClusterClientHelperWithStringKey<IProcessingGrain> processingGrainClient,
        IMapper mapper)
    {
        _arenaGrainClient = arenaGrainClient;
        _processingGrainClient = processingGrainClient;
        _mapper = mapper;
    }

    public async Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request)
    {
        var bots = await _arenaGrainClient.FromGrain(request.ArenaName, g => g.GetAllActiveBots());
        await _processingGrainClient.FromGrain(request.ArenaName, g => g.Ping());
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