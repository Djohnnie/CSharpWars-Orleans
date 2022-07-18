using CSharpWars.Common.Helpers;
using CSharpWars.Orleans.Host;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })

    .ConfigureServices(services =>
    {
        services.AddCommonHelpers();
        services.AddHostedService<Worker>();
    })

    .UseOrleans((hostBuilder, siloBuilder) =>
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        var applicationInsightsInstrumentationKey = hostBuilder.Configuration.GetValue<string>("APPLICATION_INSIGHTS_INSTRUMENTATION_KEY");

#if DEBUG
        siloBuilder.UseLocalhostClustering();
        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "my-first-cluster";
            options.ServiceId = "CSharpWarsOrleansService";
        });
#else
        siloBuilder.UseKubernetesHosting();

        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.ConfigureTableServiceClient(azureStorageConnectionString);
        });
#endif

        siloBuilder.ConfigureApplicationParts(c =>
        {
            c.AddFromApplicationBaseDirectory();
        });

        siloBuilder.AddAzureBlobGrainStorage(
            name: "arenaStore",
            configureOptions: options =>
            {
                options.UseJson = true;
                options.ConfigureBlobServiceClient(azureStorageConnectionString);
            });
        siloBuilder.AddAzureBlobGrainStorage(
            name: "playerStore",
            configureOptions: options =>
            {
                options.UseJson = true;
                options.ConfigureBlobServiceClient(azureStorageConnectionString);
            });
        siloBuilder.AddAzureBlobGrainStorage(
            name: "botStore",
            configureOptions: options =>
            {
                options.UseJson = true;
                options.ConfigureBlobServiceClient(azureStorageConnectionString);
            });

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
            o.CollectionAge = TimeSpan.FromMinutes(2);
        });
    })
    .Build();

await host.RunAsync();