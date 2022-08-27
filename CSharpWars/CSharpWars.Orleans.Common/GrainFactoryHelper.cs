using Orleans;

namespace CSharpWars.Orleans.Common;

public interface IGrainFactoryHelper<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    Task FromGrain(Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(Func<TGrainInterface, Task<TResult>> execute);
}

public interface IGrainFactoryHelperWithGuidKey<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    Task FromGrain(Guid key, Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(Guid key, Func<TGrainInterface, Task<TResult>> execute);
}

public interface IGrainFactoryHelperWithStringKey<TGrainInterface> where TGrainInterface : IGrainWithStringKey
{
    Task FromGrain(string key, Func<TGrainInterface, Task> execute);
    Task<TResult> FromGrain<TResult>(string key, Func<TGrainInterface, Task<TResult>> execute);
}

public class GrainFactoryHelper<TGrainInterface> : IGrainFactoryHelper<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    private readonly IClusterClient _clusterClient;

    public GrainFactoryHelper(IClusterClient clusterClient)
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

public class GrainFactoryHelperWithGuidKey<TGrainInterface> : IGrainFactoryHelperWithGuidKey<TGrainInterface> where TGrainInterface : IGrainWithGuidKey
{
    private readonly IClusterClient _clusterClient;

    public GrainFactoryHelperWithGuidKey(IClusterClient clusterClient)
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

public class GrainFactoryHelperWithStringKey<TGrainInterface> : IGrainFactoryHelperWithStringKey<TGrainInterface> where TGrainInterface : IGrainWithStringKey
{
    private readonly IClusterClient _clusterClient;

    public GrainFactoryHelperWithStringKey(IClusterClient clusterClient)
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