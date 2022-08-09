using AutoMapper;
using CSharpWars.Orleans.Contracts.Status;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Extensions;
using CSharpWars.WebApi.Helpers;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IStatusManager
{
    Task<GetStatusResponse> GetStatus();
}

public class StatusManager : IStatusManager
{
    private readonly IClusterClientHelper<IStatusGrain> _clusterClientHelper;
    private readonly IMapper _mapper;

    public StatusManager(
        IClusterClientHelper<IStatusGrain> clusterClientHelper,
        IMapper mapper)
    {
        _clusterClientHelper = clusterClientHelper;
        _mapper = mapper;
    }

    public async Task<GetStatusResponse> GetStatus()
    {
        var status = await _clusterClientHelper.FromGrain(g => g.GetStatus());
        return _mapper.Map<GetStatusResponse>(status);
    }
}