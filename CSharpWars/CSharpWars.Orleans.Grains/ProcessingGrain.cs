using CSharpWars.Orleans.Grains.Logic;
using Orleans;

namespace CSharpWars.Orleans.Grains;

public interface IProcessingGrain : IGrainWithStringKey
{
    Task Ping();

    Task Stop();
}

public class ProcessingGrain : Grain, IProcessingGrain
{
    private readonly IProcessorLogic _processorLogic;

    private IDisposable? _timer;

    public ProcessingGrain(IProcessorLogic processorLogic)
    {
        _processorLogic = processorLogic;
    }

    public override Task OnActivateAsync()
    {
        _timer = RegisterTimer(OnTimer, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));

        return base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
        _timer?.Dispose();

        return base.OnDeactivateAsync();
    }

    private async Task OnTimer(object state)
    {
        var arenaName = this.GetPrimaryKeyString();

        await _processorLogic.Go(arenaName);
    }

    public Task Ping()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        _timer?.Dispose();

        DeactivateOnIdle();

        return Task.CompletedTask;
    }
}