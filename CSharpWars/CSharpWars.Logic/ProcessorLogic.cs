using Orleans;

namespace CSharpWars.Logic;

public interface IProcessorLogic
{
    Task Go();
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
    }
}