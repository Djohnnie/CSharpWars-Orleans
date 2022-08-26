using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Contracts.Status;
using CSharpWars.Orleans.Grains.Base;
using Microsoft.Extensions.Logging;
using Orleans;

namespace CSharpWars.Orleans.Grains;


public interface IStatusGrain : IGrainWithGuidKey
{
    Task<StatusDto> GetStatus();
}

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

        return Task.FromResult(new StatusDto(message));
    }
}