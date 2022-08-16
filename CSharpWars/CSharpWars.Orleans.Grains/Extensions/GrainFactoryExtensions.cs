using Orleans;

namespace CSharpWars.Orleans.Grains.Extensions;

public static class GrainFactoryExtensions
{
    public static TGrainInterface GetGrain<TGrainInterface>(this IGrainFactory grainFactory) where TGrainInterface : IGrainWithGuidKey
    {
        return grainFactory.GetGrain<TGrainInterface>(Guid.Empty);
    }
}