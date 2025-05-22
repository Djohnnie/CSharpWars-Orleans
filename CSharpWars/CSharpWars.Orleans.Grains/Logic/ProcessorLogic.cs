using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Scripting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IProcessorLogic
{
    Task Go(string arenaName);
}

public class ProcessorLogic : IProcessorLogic
{
    private readonly IGrainFactoryHelperWithStringKey<IArenaGrain> _arenaGrainFactory;
    private readonly IGrainFactoryHelperWithGuidKey<IBotGrain> _botGrainFactory;
    private readonly IPreprocessingLogic _preprocessingLogic;
    private readonly IProcessingLogic _processingLogic;
    private readonly IPostprocessingLogic _postprocessingLogic;
    private readonly ILogger<ProcessorLogic> _logger;

    public ProcessorLogic(
        IGrainFactoryHelperWithStringKey<IArenaGrain> arenaGrainFactory,
        IGrainFactoryHelperWithGuidKey<IBotGrain> botGrainFactory,
        IPreprocessingLogic preprocessingLogic,
        IProcessingLogic processingLogic,
        IPostprocessingLogic postprocessingLogic,
        ILogger<ProcessorLogic> logger)
    {
        _arenaGrainFactory = arenaGrainFactory;
        _botGrainFactory = botGrainFactory;
        _preprocessingLogic = preprocessingLogic;
        _processingLogic = processingLogic;
        _postprocessingLogic = postprocessingLogic;
        _logger = logger;
    }

    public async Task Go(string arenaName)
    {
        var startTime = Stopwatch.GetTimestamp();

        var (arena, allBots, bots) = await _arenaGrainFactory.FromGrain(arenaName, async arenaGrain =>
        {
            var arenaDetails = await arenaGrain.GetArenaDetails();
            var activeBots = await arenaGrain.GetAllActiveBots();
            var liveBots = await arenaGrain.GetAllLiveBots();
            return (arenaDetails, activeBots, liveBots);
        });

        var context = ProcessingContext.Build(arena, bots);

        _logger.LogInformation($"PREPARE: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");
        startTime = Stopwatch.GetTimestamp();

        await _preprocessingLogic.Go(context);

        _logger.LogInformation($"PREPROCESSOR: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");
        startTime = Stopwatch.GetTimestamp();

        await _processingLogic.Go(context);

        _logger.LogInformation($"PROCESSOR: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");
        startTime = Stopwatch.GetTimestamp();

        await _postprocessingLogic.Go(context);

        _logger.LogInformation($"POSTPROCESSOR: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");
        startTime = Stopwatch.GetTimestamp();

        var botsToDelete = allBots.Where(x => x.TimeOfDeath < DateTime.UtcNow.AddSeconds(-10)).Select(x => x.BotId).ToArray();

        await _arenaGrainFactory.FromGrain(arenaName, async arenaGrain =>
        {
            await arenaGrain.DeleteBots(botsToDelete);
        });

        var updateStateTasks = new List<Task>();

        for (int i = 0; i < context.Bots.Count; i++)
        {
            Contracts.BotDto? bot = context.Bots[i];
            updateStateTasks.Add(_botGrainFactory.FromGrain(bot.BotId, g => g.UpdateState(bot)));
        }

        await Task.WhenAll(updateStateTasks);

        _logger.LogInformation($"UPDATE: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:F0}ms");
    }
}