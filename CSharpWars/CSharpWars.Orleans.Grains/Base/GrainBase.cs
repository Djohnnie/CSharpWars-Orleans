using CSharpWars.Common.Extensions;
using Microsoft.Extensions.Logging;
using Orleans;

namespace CSharpWars.Orleans.Grains.Base;

public class GrainBase<TGrainInterface> : Grain
{
    private readonly ILogger<TGrainInterface> _logger;

    public GrainBase(ILogger<TGrainInterface> logger)
    {
        _logger = logger;
    }

    public override Task OnActivateAsync()
    {
        _logger.AutoLogInformation("Activating Grain...");

        return base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
        _logger.AutoLogInformation("Deactivating Grain...");

        return base.OnDeactivateAsync();
    }
}