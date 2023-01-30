using CSharpWars.Orleans.Contracts.Model;
using CSharpWars.Scripting;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IPreprocessingLogic
{
    Task Go(ProcessingContext context);
}

public class PreprocessingLogic : IPreprocessingLogic
{
    public Task Go(ProcessingContext context)
    {
        for (int i = 0; i < context.Bots.Count; i++)
        {
            Contracts.BotDto bot = context.Bots[i];
            var botProperties = BotProperties.Build(bot, context.Arena, context.Bots);
            context.AddBotProperties(bot.BotId, botProperties);
        }

        return Task.CompletedTask;
    }
}