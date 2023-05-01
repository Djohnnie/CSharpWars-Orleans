using CSharpWars.Orleans.Common;
using CSharpWars.Web.Client;
using Orleans.Configuration;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;

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

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddOrleansHelpers();

builder.Services.AddClients();

builder.Host.UseOrleans((hostBuilder, siloBuilder) =>
{
    var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
    var shouldUseKubernetes = hostBuilder.Configuration.GetValue<bool>("USE_KUBERNETES");

#if DEBUG
    siloBuilder.UseLocalhostClustering(siloPort: 11114, gatewayPort: 30003, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans-host", clusterId: "csharpwars-orleans-host");
#else
    if( shouldUseKubernetes)
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

    siloBuilder.UseDashboard();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles(new StaticFileOptions
{
    //ContentTypeProvider = contentTypeProvider,
    OnPrepareResponse = context =>
    {
        IHeaderDictionary headers = context.Context.Response.Headers;
        string contentType = headers["Content-Type"];
        if (contentType == "application/x-gzip")
        {
            if (context.File.Name.EndsWith("js.gz"))
            {
                contentType = "application/javascript";
            }
            else if (context.File.Name.EndsWith("css.gz"))
            {
                contentType = "text/css";
            }
            headers.Add("Content-Encoding", "gzip");
            headers["Content-Type"] = contentType;
        }

        if (context.Context.Request.Path.Value.EndsWith("wasm.gz"))
        {
            headers["Content-Type"] = "application/wasm";
        }
    }
});

app.UseResponseCompression();
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