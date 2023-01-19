using CSharpWars.Scripting;
using System.ComponentModel.Design.Serialization;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IProcessingLogic
{
    Task Go(ProcessingContext context);
}

public class ProcessingLogic : IProcessingLogic
{
    private readonly IGrainFactoryHelperWithGuidKey<IScriptGrain> _scriptGrainFactory;

    public ProcessingLogic(IGrainFactoryHelperWithGuidKey<IScriptGrain> scriptGrainFactory)
    {
        _scriptGrainFactory = scriptGrainFactory;
    }

    public async Task Go(ProcessingContext context)
    {
        var tasks = new Dictionary<Guid, Task<BotProperties>>();

        for (int i = 0; i < context.Bots.Count; i++)
        {
            Contracts.BotDto? bot = context.Bots[i];
            var botProperties = context.GetBotProperties(bot.BotId);
            var task = _scriptGrainFactory.FromGrain(bot.BotId, g => g.Process(botProperties));
            tasks.Add(bot.BotId, task);
        }

        await Task.WhenAll(tasks.Values);

        foreach (var task in tasks.Values)
        {
            context.UpdateBotProperties(task.Result);
        }
    }
}