using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CSharpWars.Common.Helpers;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Grains.Logic;
using CSharpWars.Scripting;
using Orleans.Dashboard;
using Orleans.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddServiceDefaults();

builder.Services.AddCommonHelpers();
builder.Services.AddScripting();
builder.Services.AddOrleansHelpers();
builder.Services.AddGrainLogic();

builder.UseOrleans(siloBuilder =>
{
    var useAspire = builder.Configuration.GetValue<bool>("USE_ASPIRE");

    if (useAspire)
    {
        siloBuilder.UseLocalhostClustering(
            siloPort: 11112,
            gatewayPort: 30001,
            primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112),
            serviceId: "csharpwars-orleans",
            clusterId: "csharpwars-orleans");

        siloBuilder.AddMemoryGrainStorage("arenaStore");
        siloBuilder.AddMemoryGrainStorage("playersStore");
        siloBuilder.AddMemoryGrainStorage("playerStore");
        siloBuilder.AddMemoryGrainStorage("botStore");
        siloBuilder.AddMemoryGrainStorage("scriptStore");
        siloBuilder.AddMemoryGrainStorage("messagesStore");
        siloBuilder.AddMemoryGrainStorage("movesStore");
        siloBuilder.AddMemoryGrainStorage("tickStore");
    }
    else
    {
        var azureStorageConnectionString = builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var shouldUseKubernetes = builder.Configuration.GetValue<bool>("USE_KUBERNETES");

        if (shouldUseKubernetes)
        {
            siloBuilder.UseKubernetesHosting();
        }

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
        siloBuilder.AddAzureBlobGrainStorage("tickStore", config => config.BlobServiceClient = new BlobServiceClient(azureStorageConnectionString));
    }

    siloBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "csharpwars-orleans";
        options.ServiceId = "csharpwars-orleans";
    });

    siloBuilder.UseTransactions();
    siloBuilder.AddDashboard();

    siloBuilder.Configure<GrainCollectionOptions>(options =>
    {
        options.CollectionAge = TimeSpan.FromMinutes(10);
        options.CollectionQuantum = TimeSpan.FromMinutes(5);
    });
});

var app = builder.Build();

app.MapOrleansDashboard("/");
app.MapDefaultEndpoints();

await app.RunAsync();