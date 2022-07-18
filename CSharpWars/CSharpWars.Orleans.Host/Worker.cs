using CSharpWars.Orleans.Grains;
using Orleans;

namespace CSharpWars.Orleans.Host;

public class Worker : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IGrainFactory grainFactory,
        ILogger<Worker> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var grain = _grainFactory.GetGrain<IArenaGrain>("dummy");

                var arena = await grain.GetArenaDetails();

                _logger.LogInformation(arena.Name);

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}