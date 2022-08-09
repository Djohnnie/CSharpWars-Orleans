using AutoMapper;
using CSharpWars.Orleans.Contracts.Bot;
using CSharpWars.Orleans.Grains;
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
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _clusterClientHelper;
    private readonly IMapper _mapper;

    public BotManager(
        IClusterClientHelperWithStringKey<IArenaGrain> clusterClientHelper,
        IMapper mapper)
    {
        _clusterClientHelper = clusterClientHelper;
        _mapper = mapper;
    }

    public async Task<GetAllActiveBotsResponse> GetAllActiveBots(GetAllActiveBotsRequest request)
    {
        var bots = await _clusterClientHelper.FromGrain(request.ArenaName, g => g.GetAllActiveBots());
        return _mapper.Map<GetAllActiveBotsResponse>(bots);
    }

    public async Task<CreateBotResponse> CreateBot(CreateBotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerName))
        {
            throw new ArgumentNullException(nameof(request.PlayerName));
        }

        var botToCreate = _mapper.Map<BotToCreateDto>(request);

        var bot = await _clusterClientHelper.FromGrain(request.ArenaName, g => g.CreateBot(request.PlayerName, botToCreate));
        return _mapper.Map<CreateBotResponse>(bot);
    }
}