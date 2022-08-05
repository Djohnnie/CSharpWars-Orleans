using Orleans;

namespace CSharpWars.Logic;

public interface IPreprocessingLogic
{
    Task Go();
}

public class PreprocessingLogic : IPreprocessingLogic
{
    private readonly IGrainFactory _grainFactory;

    public PreprocessingLogic(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public Task Go()
    {
        
    }
}