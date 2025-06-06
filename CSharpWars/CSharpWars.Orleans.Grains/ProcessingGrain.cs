﻿using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Orleans.Grains.Logic;
using Microsoft.Extensions.Logging;

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

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _timer = this.RegisterGrainTimer(OnTimer, new() { DueTime = TimeSpan.FromSeconds(5), Period = TimeSpan.FromSeconds(2) });

        return base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _timer?.Dispose();

        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    private async Task OnTimer(CancellationToken cancellationToken)
    {
        try
        {
            var arenaName = this.GetPrimaryKeyString();
            _logger.AutoLogInformation($"INFO: {nameof(ProcessingGrain)} timer for '{this.GetPrimaryKeyString()}' has ticked");

            await _processorLogic.Go(arenaName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR: {nameof(ProcessingGrain)} timer for '{this.GetPrimaryKeyString()}' has failed");
        }
    }

    public Task Ping()
    {
        _logger.AutoLogInformation($"INFO: {nameof(ProcessingGrain)} for '{this.GetPrimaryKeyString()}' was pinged");

        return Task.CompletedTask;
    }

    public Task Stop()
    {
        _timer?.Dispose();

        DeactivateOnIdle();

        return Task.CompletedTask;
    }
}