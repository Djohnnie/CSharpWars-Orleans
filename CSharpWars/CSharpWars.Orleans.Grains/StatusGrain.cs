using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Orleans.Contracts.Status;
using CSharpWars.Orleans.Grains.Base;
using Microsoft.Extensions.Logging;

namespace CSharpWars.Orleans.Grains;

public class StatusGrain : GrainBase<IStatusGrain>, IStatusGrain
{
    private readonly ILogger<IStatusGrain> _logger;

    public StatusGrain(ILogger<IStatusGrain> logger) : base(logger)
    {
        _logger = logger;
    }

    public async Task<StatusDto> GetStatus()
    {
        var message = "Hi from the <StatusGrain>";
        _logger.AutoLogInformation(message);

        return new StatusDto(message);
    }
}