using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Orleans.Grains.Logic;
using Microsoft.Extensions.Logging;
using Orleans;

namespace CSharpWars.Orleans.Grains;

public class ProcessingGrain : GrainBase<IProcessingGrain>, IProcessingGrain
{
    private readonly ILogger<IProcessingGrain> _logger;
    private readonly IProcessorLogic _processorLogic;

    private IDisposable? _timer;

    public ProcessingGrain(
        ILogger<IProcessingGrain> logger,
        IProcessorLogic processorLogic) : base(logger)
    {
        _logger = logger;
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
        _logger.AutoLogInformation($"{nameof(ProcessingGrain)} timer for '{this.GetPrimaryKeyString()}' has ticked");

        await _processorLogic.Go(arenaName);
    }

    public Task Ping()
    {
        _logger.AutoLogInformation($"{nameof(ProcessingGrain)} for '{this.GetPrimaryKeyString()}' was pinged");

        return Task.CompletedTask;
    }

    public Task Stop()
    {
        _timer?.Dispose();

        DeactivateOnIdle();

        return Task.CompletedTask;
    }
}