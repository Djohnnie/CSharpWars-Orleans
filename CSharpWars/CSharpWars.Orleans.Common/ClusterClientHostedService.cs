using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace CSharpWars.Orleans.Common;

public class ClusterClientHostedService : IHostedService
{
    public IClusterClient Client { get; }

    public ClusterClientHostedService(
        IConfiguration configuration,
        ILoggerProvider loggerProvider)
    {
        Client = new ClientBuilder()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "csharpwars-orleans-host";
                options.ServiceId = "csharpwars-orleans-host";
            })
            .UseAzureStorageClustering(options =>
            {
                options.ConfigureTableServiceClient(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING"));
            })

            .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Client.Connect();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.Close();

        Client.Dispose();
    }
}