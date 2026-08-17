using Azure.Data.Tables;
using CSharpWars.Orleans.Common;
using CSharpWars.Scripting;
using Orleans.Dashboard;
using Orleans.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddServiceDefaults();

builder.Services.AddOrleansHelpers();
builder.Services.AddScripting();

builder.UseOrleans(siloBuilder =>
{
    var useAspire = builder.Configuration.GetValue<bool>("USE_ASPIRE");

    if (useAspire)
    {
        siloBuilder.UseLocalhostClustering(
            siloPort: 11111,
            gatewayPort: 30000,
            primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112),
            serviceId: "csharpwars-orleans",
            clusterId: "csharpwars-orleans");
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
    }

    siloBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "csharpwars-orleans";
        options.ServiceId = "csharpwars-orleans";
    });

    siloBuilder.ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    });

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