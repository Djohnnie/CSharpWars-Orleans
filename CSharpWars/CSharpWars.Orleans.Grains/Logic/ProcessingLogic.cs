using CSharpWars.Scripting;
using Orleans;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IProcessingLogic
{
    Task Go(ProcessingContext context);
}

public class ProcessingLogic : IProcessingLogic
{
    private readonly IGrainFactory _grainFactory;

    public ProcessingLogic(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task Go(ProcessingContext context)
    {
        var tasks = new Dictionary<Guid, Task<bool>>();

        foreach (var bot in context.Bots)
        {
            var botGrain = _grainFactory.GetGrain<IBotGrain>(bot.BotId);
            tasks.Add(bot.BotId, botGrain.Process());
        }

        await Task.WhenAll(tasks.Values);

        foreach (var processTask in tasks)
        {
            if (!processTask.Value.Result)
            {
                //_state.State.BotIds.Remove(processTask.Key);
            }
        }
    }
}