using CSharpWars.Orleans.Common;
using CSharpWars.Scripting;
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
        services.AddOrleansHelpers();
        services.AddScripting();
    })

    .UseOrleans((hostBuilder, siloBuilder) =>
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var applicationInsightsConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_CONNECTION_STRING");
        var applicationInsightsInstrumentationKey = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_INSTRUMENTATION_KEY");

#if DEBUG
        siloBuilder.UsePerfCounterEnvironmentStatistics();
        siloBuilder.ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000);
#else
        siloBuilder.UseKubernetesHosting();
        siloBuilder.UseLinuxEnvironmentStatistics();
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

        siloBuilder.AddApplicationInsightsTelemetryConsumer(applicationInsightsInstrumentationKey);
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