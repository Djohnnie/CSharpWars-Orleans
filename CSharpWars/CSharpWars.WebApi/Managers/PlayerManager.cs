using AutoMapper;
using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IPlayerManager
{
    Task<LoginResponse> Login(LoginRequest request);
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
        var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(request.Username);
        var player = await playerGrain.Login(request.Username, request.Password);
        return _mapper.Map<LoginResponse>(player);
    }
}