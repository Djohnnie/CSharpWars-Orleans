namespace CSharpWars.Logic;

public interface IProcessingLogic
{
    Task Go();
}

public class ProcessingLogic : IProcessingLogic
{
    public Task Go()
    {
        throw new NotImplementedException();
    }
}