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

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddOrleansHelpers();

builder.Services.AddClients();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles(new StaticFileOptions
{
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