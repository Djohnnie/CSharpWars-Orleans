using CSharpWars.Common.Extensions;
using Microsoft.Extensions.Logging;
using Orleans;

namespace CSharpWars.Orleans.Common;

public class GrainBase<TGrainInterface> : Grain
{
    private readonly ILogger<TGrainInterface> _logger;

    public GrainBase(ILogger<TGrainInterface> logger)
    {
        _logger = logger;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _logger.AutoLogInformation("Activating Grain...");

        return base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _logger.AutoLogInformation("Deactivating Grain...");

        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}