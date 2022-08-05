using CSharpWars.Common.Helpers;
using CSharpWars.Orleans.Grains.Logic;
using CSharpWars.Orleans.Host;
using CSharpWars.Orleans.Host.Extensions;
using CSharpWars.Scripting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })

    .ConfigureServices(services =>
    {
        services.AddCommonHelpers();
        services.AddScripting();
        services.AddGrainLogic();
        services.AddHostedService<Worker>();
    })

    .UseOrleans((hostBuilder, siloBuilder) =>
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var applicationInsightsInstrumentationKey = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_INSTRUMENTATION_KEY");

#if DEBUG
        siloBuilder.UseLocalhostClustering();
        siloBuilder.UsePerfCounterEnvironmentStatistics();

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "my-first-cluster";
            options.ServiceId = "CSharpWarsOrleansService";
        });
#else
        siloBuilder.UseKubernetesHosting();
        siloBuilder.UseLinuxEnvironmentStatistics();

        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.ConfigureTableServiceClient(azureStorageConnectionString);
        });
#endif

        siloBuilder.ConfigureApplicationParts(c =>
        {
            c.AddFromApplicationBaseDirectory();
        });

        siloBuilder.AddAzureBlobGrainStorage("arenaStore", azureStorageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("playersStore", azureStorageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("playerStore", azureStorageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("botStore", azureStorageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("scriptStore", azureStorageConnectionString);

        siloBuilder.AddMemoryGrainStorage("urls");

        siloBuilder.AddApplicationInsightsTelemetryConsumer(applicationInsightsInstrumentationKey);
        siloBuilder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddApplicationInsights(applicationInsightsInstrumentationKey);
        });

        siloBuilder.UseDashboard();

        siloBuilder.Configure<GrainCollectionOptions>(o =>
        {
            o.CollectionAge = TimeSpan.FromHours(1);
            o.CollectionQuantum = TimeSpan.FromMinutes(10);
        });
    })
    .Build();

await host.RunAsync();