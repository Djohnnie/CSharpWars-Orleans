using CSharpWars.Orleans.Common;
using CSharpWars.Scripting;
using Orleans.Configuration;
using System.Net;

IHost host = Host.CreateDefaultBuilder(args)

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
        siloBuilder.UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans-host", clusterId: "csharpwars-orleans-host");
#else
        if(shouldUseKubernetes)
        {
            siloBuilder.UseKubernetesHosting();
        }
#endif

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "csharpwars-orleans-host";
            options.ServiceId = "csharpwars-orleans-host";
        });

        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.ConfigureTableServiceClient(azureStorageConnectionString);
        });

        siloBuilder.AddAzureBlobGrainStorage("arenaStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("playersStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("playerStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("botStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("scriptStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("messagesStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("movesStore", config => config.ConfigureBlobServiceClient(azureStorageConnectionString));

        siloBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddApplicationInsights(c => c.ConnectionString = applicationInsightsConnectionString, _ => { });
        });

        siloBuilder.Configure<GrainCollectionOptions>(o =>
        {
            o.CollectionAge = TimeSpan.FromMinutes(10);
            o.CollectionQuantum = TimeSpan.FromMinutes(5);
        });
    })
    .Build();

await host.RunAsync();