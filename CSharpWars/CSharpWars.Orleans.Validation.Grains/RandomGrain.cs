using CSharpWars.Orleans.Contracts.Grains;
using Orleans;

namespace CSharpWars.Orleans.Validation.Grains;

public class RandomGrain : Grain, IRandomGrain
{
    public Task Do()
    {
        return Task.CompletedTask;
    }
}