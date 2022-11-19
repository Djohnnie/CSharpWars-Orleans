using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.WebApi.Managers;

public interface IStatusManager
{
    Task<GetStatusResponse> GetStatus();
}

public class StatusManager : IStatusManager
{
    private readonly IGrainFactoryHelper<IStatusGrain> _statusGrainClient;
    private readonly IMapper _mapper;

    public StatusManager(
        IGrainFactoryHelper<IStatusGrain> statusGrainClient,
        IMapper mapper)
    {
        _statusGrainClient = statusGrainClient;
        _mapper = mapper;
    }

    public async Task<GetStatusResponse> GetStatus()
    {
        var status = await _statusGrainClient.FromGrain(g => g.GetStatus());
        return _mapper.Map<GetStatusResponse>(status);
    }
}