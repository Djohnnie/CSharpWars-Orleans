using CSharpWars.Orleans.Scaler.Services;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddGrpc();

builder.Host.UseOrleansClient((hostBuilder, clientBuilder) =>
{
    var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

    clientBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "csharpwars-orleans";
        options.ServiceId = "csharpwars-orleans";
    });

#if DEBUG
    clientBuilder.UseLocalhostClustering(gatewayPort: 30001, clusterId: "csharpwars-orleans", serviceId: "csharpwars-orleans");
#else
    clientBuilder.UseAzureStorageClustering(options =>
    {
        options.ConfigureTableServiceClient(azureStorageConnectionString);
    });
#endif
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ExternalScalerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();