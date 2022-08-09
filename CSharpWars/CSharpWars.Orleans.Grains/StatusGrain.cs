using CSharpWars.Orleans.Contracts.Status;
using Orleans;

namespace CSharpWars.Orleans.Grains;


public interface IStatusGrain : IGrainWithGuidKey
{
    Task<StatusDto> GetStatus();
}

public class StatusGrain : Grain, IStatusGrain
{
    public Task<StatusDto> GetStatus()
    {
        var message = "Hi from the <StatusGrain>";
        return Task.FromResult(new StatusDto(message));
    }
}