using CSharpWars.Orleans.Contracts.Status;
using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IStatusGrain : IGrainWithGuidKey
{
    Task<StatusDto> GetStatus();
}