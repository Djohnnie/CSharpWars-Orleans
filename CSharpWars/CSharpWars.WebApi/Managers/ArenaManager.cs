using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;
using System.Diagnostics;

namespace CSharpWars.WebApi.Managers;

public interface IArenaManager
{
    Task<GetArenaResponse> GetArena(GetArenaRequest request);
    Task DeleteArena(string arenaName);
}

public class ArenaManager : IArenaManager
{
    private readonly IGrainFactoryHelperWithStringKey<IArenaGrain> _arenaGrainClient;
    private readonly IGrainFactoryHelperWithStringKey<IProcessingGrain> _processingGrainClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ArenaManager> _logger;

    public ArenaManager(
        IGrainFactoryHelperWithStringKey<IArenaGrain> arenaGrainClient,
        IGrainFactoryHelperWithStringKey<IProcessingGrain> processingGrainClient,
        IMapper mapper, ILogger<ArenaManager> logger)
    {
        _arenaGrainClient = arenaGrainClient;
        _processingGrainClient = processingGrainClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetArenaResponse> GetArena(GetArenaRequest request)
    {
        try
        {
            var startTime = Stopwatch.GetTimestamp();

            var arena = await _arenaGrainClient.FromGrain(request.Name, g => g.GetArenaDetails());
            await _processingGrainClient.FromGrain(request.Name, g => g.Ping());

            _logger.LogInformation($"GetArena: {Stopwatch.GetElapsedTime(startTime)} ms");

            return _mapper.Map<GetArenaResponse>(arena);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR: error getting arena details");
            throw;
        }
    }

    public async Task DeleteArena(string arenaName)
    {
        await _arenaGrainClient.FromGrain(arenaName, g => g.DeleteArena());
    }
}