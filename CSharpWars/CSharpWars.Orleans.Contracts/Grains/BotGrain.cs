using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IBotGrain : IGrainWithGuidKey
{
    Task<BotDto> GetState();

    Task<BotDto> CreateBot(BotToCreateDto bot);

    Task DeleteBot(bool clearArena);

    Task UpdateState(BotDto bot);
}