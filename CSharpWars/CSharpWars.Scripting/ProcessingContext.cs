using CSharpWars.Orleans.Contracts.Arena;
using CSharpWars.Orleans.Contracts.Bot;
using CSharpWars.Scripting.Model;
using System.Collections.Concurrent;

namespace CSharpWars.Scripting;

public class ProcessingContext
{
    private readonly ConcurrentDictionary<Guid, BotProperties> _botProperties = new();

    public ArenaDto Arena { get; }
    public IList<BotDto> Bots { get; }

    private ProcessingContext(ArenaDto arena, IList<BotDto> bots)
    {
        Arena = arena;
        Bots = bots;
    }

    public static ProcessingContext Build(ArenaDto arena, IList<BotDto> bots)
    {
        return new ProcessingContext(arena, bots);
    }

    public void AddBotProperties(Guid botId, BotProperties botProperties)
    {
        _botProperties.TryAdd(botId, botProperties);
    }

    public BotProperties GetBotProperties(Guid botId)
    {
        return _botProperties[botId];
    }

    public IEnumerable<BotProperties> GetOrderedBotProperties()
    {
        return _botProperties.Values.OrderBy(x => x.CurrentMove, new MoveComparer());
    }

    public void UpdateBotProperties(BotDto bot)
    {
        foreach (var botProperties in _botProperties.Values)
        {
            var botToUpdate = botProperties.Bots.Single(x => x.Id == bot.BotId);
            botToUpdate.Update(bot);
        }

        var botPropertiesToUpdate = _botProperties.Values.Single(x => x.BotId == bot.BotId);
        botPropertiesToUpdate.Update(bot);
    }
}