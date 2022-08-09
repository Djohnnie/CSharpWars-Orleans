using Orleans;

namespace CSharpWars.WebApi.Extensions;

public static class ClusterClientExtensions
{
    public static TGrainInterface GetGrain<TGrainInterface>(this IClusterClient clusterClient) where TGrainInterface : IGrainWithGuidKey
    {
        return clusterClient.GetGrain<TGrainInterface>(Guid.Empty);
    }

    public static async Task<TResult> FromGrain<TGrainInterface, TResult>(this IClusterClient clusterClient, Func<TGrainInterface, Task<TResult>> execute) where TGrainInterface : IGrainWithGuidKey
    {
        var grain = clusterClient.GetGrain<TGrainInterface>();
        return await execute(grain);
    }
}