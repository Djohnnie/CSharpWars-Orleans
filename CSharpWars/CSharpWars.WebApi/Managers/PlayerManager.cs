using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IPlayerManager
{
    Task<LoginResponse> Login(LoginRequest request);
    Task DeleteAllPlayers();
}

public class PlayerManager : IPlayerManager
{
    private readonly IMapper _mapper;
    private readonly IClusterClient _clusterClient;

    public PlayerManager(
        IMapper mapper,
        IClusterClient clusterClient)
    {
        _mapper = mapper;
        _clusterClient = clusterClient;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var playersGrain = _clusterClient.GetGrain<IPlayersGrain>(nameof(PlayerManager));
        var player = await playersGrain.Login(request.Username, request.Password);
        return _mapper.Map<LoginResponse>(player);
    }

    public async Task DeleteAllPlayers()
    {
        var playersGrain = _clusterClient.GetGrain<IPlayersGrain>(nameof(PlayerManager));
        await playersGrain.DeleteAllPlayers();
    }
}