using CSharpWars.Orleans.Common;
using CSharpWars.Web.Client;
using Orleans.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.WebHost.UseKestrel();

builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddOrleansHelpers();

builder.Services.AddClients();

builder.Host.UseOrleans((hostBuilder, siloBuilder) =>
{
    var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

#if DEBUG
    //siloBuilder.UsePerfCounterEnvironmentStatistics();
    siloBuilder.UseLocalhostClustering(siloPort: 11114, gatewayPort: 30003, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans-host", clusterId: "csharpwars-orleans-host");
#else
    siloBuilder.UseKubernetesHosting();
    //siloBuilder.UseLinuxEnvironmentStatistics();
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
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#if DEBUG
await Task.Delay(30000);
#endif

app.Run();