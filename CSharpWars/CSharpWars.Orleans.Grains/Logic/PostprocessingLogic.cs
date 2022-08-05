using CSharpWars.Scripting;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IPostprocessingLogic
{
    Task Go(ProcessingContext context);
}

public class PostprocessingLogic : IPostprocessingLogic
{
    public Task Go(ProcessingContext context)
    {
        throw new NotImplementedException();
    }
}