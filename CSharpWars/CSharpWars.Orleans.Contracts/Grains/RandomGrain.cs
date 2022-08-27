using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IRandomGrain : IGrainWithGuidKey
{
    Task Do();
}