using CSharpWars.Scripting;
using Orleans;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IProcessorLogic
{
    Task Go(string arenaName);
}

public class ProcessorLogic : IProcessorLogic
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPreprocessingLogic _preprocessingLogic;
    private readonly IProcessingLogic _processingLogic;
    private readonly IPostprocessingLogic _postprocessingLogic;

    public ProcessorLogic(
        IGrainFactory grainFactory,
        IPreprocessingLogic preprocessingLogic,
        IProcessingLogic processingLogic,
        IPostprocessingLogic postprocessingLogic)
    {
        _grainFactory = grainFactory;
        _preprocessingLogic = preprocessingLogic;
        _processingLogic = processingLogic;
        _postprocessingLogic = postprocessingLogic;
    }

    public async Task Go(string arenaName)
    {
        var arenaGrain = _grainFactory.GetGrain<IArenaGrain>(arenaName);
        var arena = await arenaGrain.GetArenaDetails();
        var bots = await arenaGrain.GetAllActiveBots();



        var context = ProcessingContext.Build(arena, bots);

        await _preprocessingLogic.Go(context);

        await _processingLogic.Go(context);

        await _postprocessingLogic.Go(context);
    }
}