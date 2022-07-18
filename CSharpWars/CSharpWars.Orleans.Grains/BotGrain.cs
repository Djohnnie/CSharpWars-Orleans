using CSharpWars.Orleans.Contracts.Bot;
using Orleans;

namespace CSharpWars.Orleans.Grains;

public interface IBotGrain : IGrainWithGuidKey
{
    Task<BotDto> GetState();

    Task<BotDto> CreateBot(BotToCreateDto bot);
}

public class BotGrain : Grain, IBotGrain
{
    public Task<BotDto> GetState()
    {
        return Task.FromResult(new BotDto());
    }

    public Task<BotDto> CreateBot(BotToCreateDto bot)
    {
        return Task.FromResult(new BotDto());
    }
}