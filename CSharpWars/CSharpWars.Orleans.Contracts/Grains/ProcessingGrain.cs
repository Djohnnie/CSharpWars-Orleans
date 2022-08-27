using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IProcessingGrain : IGrainWithStringKey
{
    Task Ping();

    Task Stop();
}