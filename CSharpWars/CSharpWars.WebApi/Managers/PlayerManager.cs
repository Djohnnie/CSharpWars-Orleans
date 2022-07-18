using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi.Contracts;
using Orleans;

namespace CSharpWars.WebApi.Managers;

public interface IPlayerManager
{
    Task Login(LoginRequest request);
}

public class PlayerManager : IPlayerManager
{
    private readonly IClusterClient _clusterClient;

    public PlayerManager(
        IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task Login(LoginRequest request)
    {
        var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(request.Username);
        await playerGrain.Login(request.Username, request.Password);
    }
}