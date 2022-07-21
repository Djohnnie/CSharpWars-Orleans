using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IArenaManager
{
    Task<GetArenaResponse> GetArena(GetArenaRequest request);
    Task DeleteArena(string arenaName);
}

public class ArenaManager : IArenaManager
{
    private readonly IClusterClient _clusterClient;
    private readonly IMapper _mapper;

    public ArenaManager(
        IClusterClient clusterClient,
        IMapper mapper)
    {
        _clusterClient = clusterClient;
        _mapper = mapper;
    }

    public async Task<GetArenaResponse> GetArena(GetArenaRequest request)
    {
        var arenaGrain = _clusterClient.GetGrain<IArenaGrain>(request.Name);
        var arena = await arenaGrain.GetArenaDetails();

        return _mapper.Map<GetArenaResponse>(arena);
    }

    public async Task DeleteArena(string arenaName)
    {
        var arenaGrain = _clusterClient.GetGrain<IArenaGrain>(arenaName);
        await arenaGrain.DeleteArena();
    }
}