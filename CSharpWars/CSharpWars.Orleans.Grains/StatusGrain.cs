using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Logging;

namespace CSharpWars.Orleans.Grains;

public class StatusGrain : GrainBase<IStatusGrain>, IStatusGrain
{
    private readonly ILogger<IStatusGrain> _logger;

    public StatusGrain(ILogger<IStatusGrain> logger) : base(logger)
    {
        _logger = logger;
    }

    public Task<StatusDto> GetStatus()
    {
        var message = "Hi from the <StatusGrain>";
        _logger.AutoLogInformation(message);

        return Task.FromResult(new StatusDto
        {
            Message = message
        });
    }
}