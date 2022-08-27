using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.WebApi.Managers;

public interface IPlayerManager
{
    Task<LoginResponse> Login(LoginRequest request);
    Task DeleteAllPlayers();
}

public class PlayerManager : IPlayerManager
{
    private readonly IClusterClientHelper<IPlayersGrain> _clusterClientHelper;
    private readonly IMapper _mapper;

    public PlayerManager(
        IClusterClientHelper<IPlayersGrain> clusterClientHelper,
        IMapper mapper)
    {
        _clusterClientHelper = clusterClientHelper;
        _mapper = mapper;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var player = await _clusterClientHelper.FromGrain(g => g.Login(request.Username, request.Password));
        return _mapper.Map<LoginResponse>(player);
    }

    public async Task DeleteAllPlayers()
    {
        await _clusterClientHelper.FromGrain(g => g.DeleteAllPlayers());
    }
}