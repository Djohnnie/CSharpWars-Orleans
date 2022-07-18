using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IStatusManager
{
    Task<GetStatusResponse> GetStatus();
}

public class StatusManager : IStatusManager
{
    private readonly IClusterClient _clusterClient;
    private readonly IMapper _mapper;

    public StatusManager(
        IClusterClient clusterClient,
        IMapper mapper)
    {
        _clusterClient = clusterClient;
        _mapper = mapper;
    }

    public async Task<GetStatusResponse> GetStatus()
    {
        var statusGrain = _clusterClient.GetGrain<IStatusGrain>("CSharpWars.Orleans.Host");
        var status = await statusGrain.GetStatus();

        return _mapper.Map<GetStatusResponse>(status);
    }
}