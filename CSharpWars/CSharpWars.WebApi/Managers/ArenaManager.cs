using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Helpers;

namespace CSharpWars.WebApi.Managers;

public interface IArenaManager
{
    Task<GetArenaResponse> GetArena(GetArenaRequest request);
    Task DeleteArena(string arenaName);
}

public class ArenaManager : IArenaManager
{
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _clusterClientHelper;
    private readonly IMapper _mapper;

    public ArenaManager(
        IClusterClientHelperWithStringKey<IArenaGrain> clusterClientHelper,
        IMapper mapper)
    {
        _clusterClientHelper = clusterClientHelper;
        _mapper = mapper;
    }

    public async Task<GetArenaResponse> GetArena(GetArenaRequest request)
    {
        var arena = await _clusterClientHelper.FromGrain(request.Name, g => g.GetArenaDetails());
        return _mapper.Map<GetArenaResponse>(arena);
    }

    public async Task DeleteArena(string arenaName)
    {
        await _clusterClientHelper.FromGrain(arenaName, g => g.DeleteArena());
    }
}