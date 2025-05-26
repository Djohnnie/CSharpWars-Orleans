using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CSharpWars.Common.Helpers;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Grains.Logic;
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
        services.AddCommonHelpers();
        services.AddScripting();
        services.AddOrleansHelpers();
        services.AddGrainLogic();
    })

    .UseOrleans((hostBuilder, siloBuilder) =>
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var applicationInsightsConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_CONNECTION_STRING");
        var shouldUseKubernetes = hostBuilder.Configuration.GetValue<bool>("USE_KUBERNETES");

#if DEBUG
        siloBuilder.UseLocalhostClustering(siloPort: 11112, gatewayPort: 30001, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans", clusterId: "csharpwars-orleans");
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

        siloBuilder.AddAzureBlobGrainStorage("arenaStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("playersStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("playerStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("botStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("scriptStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("messagesStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
        siloBuilder.AddAzureBlobGrainStorage("movesStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));

        siloBuilder.UseTransactions();
        siloBuilder.UseDashboard();

        siloBuilder.Configure<GrainCollectionOptions>(o =>
        {
            o.CollectionAge = TimeSpan.FromMinutes(10);
            o.CollectionQuantum = TimeSpan.FromMinutes(5);
        });
    })
    .Build();

await host.RunAsync();