using Orleans;

namespace CSharpWars.WebApi.Extensions;

public static class ClusterClientExtensions
{
    public static TGrainInterface GetGrain<TGrainInterface>(this IClusterClient clusterClient) where TGrainInterface : IGrainWithGuidKey
    {
        return clusterClient.GetGrain<TGrainInterface>(Guid.Empty);
    }
}