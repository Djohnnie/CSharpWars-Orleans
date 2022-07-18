using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IBotManager
{
    Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request);

    Task<CreateBotResponse> CreateBot(CreateBotRequest request);
}

public class BotManager : IBotManager
{
    private readonly IClusterClient _clusterClient;
    private readonly IMapper _mapper;

    public BotManager(
        IClusterClient clusterClient,
        IMapper mapper)
    {
        _clusterClient = clusterClient;
        _mapper = mapper;
    }

    public async Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request)
    {
        var arenaGrain = _clusterClient.GetGrain<IArenaGrain>(request.ArenaName);
        var bots = await arenaGrain.GetAllActiveBots();

        return _mapper.Map<GetAllActiveBotsResponse>(bots);
    }

    public async Task<CreateBotResponse> CreateBot(CreateBotRequest request)
    {
        var botKey = Guid.NewGuid();
        var botGrain = _clusterClient.GetGrain<IBotGrain>(botKey);

        var bot = await botGrain.CreateBot(null);

        return _mapper.Map<CreateBotResponse>(bot);
    }
}