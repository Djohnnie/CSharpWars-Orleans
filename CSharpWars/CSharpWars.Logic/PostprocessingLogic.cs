namespace CSharpWars.Logic;

public interface IPostprocessingLogic
{
    Task Go();
}

public class PostprocessingLogic : IPostprocessingLogic
{
    public Task Go()
    {
        throw new NotImplementedException();
    }
}