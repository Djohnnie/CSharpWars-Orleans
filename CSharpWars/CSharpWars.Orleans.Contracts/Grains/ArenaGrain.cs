using CSharpWars.Orleans.Contracts.Arena;
using CSharpWars.Orleans.Contracts.Bot;
using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IArenaGrain : IGrainWithStringKey
{
    Task<List<BotDto>> GetAllActiveBots();
    Task<List<BotDto>> GetAllLiveBots();
    Task<ArenaDto> GetArenaDetails();
    Task<BotDto> CreateBot(string playerName, BotToCreateDto bot);
    Task DeleteArena();
    Task DeleteBot(Guid botId);
}