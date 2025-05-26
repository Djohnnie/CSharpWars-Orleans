using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CSharpWars.Orleans.Common;
using CSharpWars.Scripting;
using Orleans.Configuration;
using System.Net;

using IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })

    .ConfigureServices(services =>
    {
        services.AddOrleansHelpers();
        services.AddScripting();
    })

    .UseOrleans((hostBuilder, siloBuilder) =>
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var applicationInsightsConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_CONNECTION_STRING");
        var shouldUseKubernetes = hostBuilder.Configuration.GetValue<bool>("USE_KUBERNETES");
        
#if DEBUG
        siloBuilder.UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans", clusterId: "csharpwars-orleans");
#else
        if (shouldUseKubernetes)
        {
            siloBuilder.UseKubernetesHosting();
        }
#endif

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "csharpwars-orleans";
            options.ServiceId = "csharpwars-orleans";
        });

        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.TableServiceClient = new TableServiceClient(azureStorageConnectionString);
        });

        siloBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            //loggingBuilder.AddApplicationInsights(c => c.ConnectionString = applicationInsightsConnectionString, _ => { });
        });

        siloBuilder.UseDashboard();

        siloBuilder.Configure<GrainCollectionOptions>(o =>
        {
            o.CollectionAge = TimeSpan.FromMinutes(10);
            o.CollectionQuantum = TimeSpan.FromMinutes(5);
        });
    })
    .Build();

await host.RunAsync();