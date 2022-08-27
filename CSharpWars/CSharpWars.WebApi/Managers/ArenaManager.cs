using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.WebApi.Managers;

public interface IArenaManager
{
    Task<GetArenaResponse> GetArena(GetArenaRequest request);
    Task DeleteArena(string arenaName);
}

public class ArenaManager : IArenaManager
{
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _arenaGrainClient;
    private readonly IClusterClientHelperWithStringKey<IProcessingGrain> _processingGrainClient;
    private readonly IMapper _mapper;

    public ArenaManager(
        IClusterClientHelperWithStringKey<IArenaGrain> arenaGrainClient,
        IClusterClientHelperWithStringKey<IProcessingGrain> processingGrainClient,
        IMapper mapper)
    {
        _arenaGrainClient = arenaGrainClient;
        _processingGrainClient = processingGrainClient;
        _mapper = mapper;
    }

    public async Task<GetArenaResponse> GetArena(GetArenaRequest request)
    {
        var arena = await _arenaGrainClient.FromGrain(request.Name, g => g.GetArenaDetails());
        await _processingGrainClient.FromGrain(request.Name, g => g.Ping());
        return _mapper.Map<GetArenaResponse>(arena);
    }

    public async Task DeleteArena(string arenaName)
    {
        await _arenaGrainClient.FromGrain(arenaName, g => g.DeleteArena());
    }
}