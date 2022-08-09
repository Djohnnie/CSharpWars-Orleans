using CSharpWars.WebApi.Extensions;
using Orleans;

namespace CSharpWars.WebApi.Helpers;

public interface IClusterClientHelper<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    Task FromGrain(Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(Func<TGrainInterface, Task<TResult>> execute);
}

public interface IClusterClientHelperWithGuidKey<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    Task FromGrain(Guid key, Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(Guid key, Func<TGrainInterface, Task<TResult>> execute);
}

public interface IClusterClientHelperWithStringKey<TGrainInterface> where TGrainInterface : IGrainWithStringKey
{
    Task FromGrain(string key, Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(string key, Func<TGrainInterface, Task<TResult>> execute);
}

public class ClusterClientHelper<TGrainInterface> : IClusterClientHelper<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    private readonly IClusterClient _clusterClient;

    public ClusterClientHelper(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task FromGrain(Func<TGrainInterface, Task> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>();
        await execute(grain);
    }

    public async Task<TResult> FromGrain<TResult>(Func<TGrainInterface, Task<TResult>> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>();
        return await execute(grain);
    }
}

public class ClusterClientHelperWithGuidKey<TGrainInterface> : IClusterClientHelperWithGuidKey<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    private readonly IClusterClient _clusterClient;

    public ClusterClientHelperWithGuidKey(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task FromGrain(Guid key, Func<TGrainInterface, Task> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>(key);
        await execute(grain);
    }

    public async Task<TResult> FromGrain<TResult>(Guid key, Func<TGrainInterface, Task<TResult>> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>(key);
        return await execute(grain);
    }
}

public class ClusterClientHelperWithStringKey<TGrainInterface> : IClusterClientHelperWithStringKey<TGrainInterface> where TGrainInterface : IGrainWithStringKey
{
    private readonly IClusterClient _clusterClient;

    public ClusterClientHelperWithStringKey(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task FromGrain(string key, Func<TGrainInterface, Task> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>(key);
        await execute(grain);
    }

    public async Task<TResult> FromGrain<TResult>(string key, Func<TGrainInterface, Task<TResult>> execute)
    {
        var grain = _clusterClient.GetGrain<TGrainInterface>(key);
        return await execute(grain);
    }
}