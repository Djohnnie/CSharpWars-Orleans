using AutoMapper;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Helpers;

namespace CSharpWars.WebApi.Managers;

public interface IStatusManager
{
    Task<GetStatusResponse> GetStatus();
}

public class StatusManager : IStatusManager
{
    private readonly IClusterClientHelper<IStatusGrain> _statusGrainClient;
    private readonly IClusterClientHelperWithGuidKey<IRandomGrain> _randomGrainClient;
    private readonly IMapper _mapper;

    public StatusManager(
        IClusterClientHelper<IStatusGrain> statusGrainClient,
        IClusterClientHelperWithGuidKey<IRandomGrain> randomGrainClient,
        IMapper mapper)
    {
        _statusGrainClient = statusGrainClient;
        _randomGrainClient = randomGrainClient;
        _mapper = mapper;
    }

    public async Task<GetStatusResponse> GetStatus()
    {
        var status = await _statusGrainClient.FromGrain(g => g.GetStatus());
        await _randomGrainClient.FromGrain(Guid.NewGuid(), g => g.Do());
        return _mapper.Map<GetStatusResponse>(status);
    }
}