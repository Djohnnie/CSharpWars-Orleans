using Azure.Data.Tables;
using CSharpWars.Orleans.Common;
using CSharpWars.Web.Client;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.AddServiceDefaults();
builder.WebHost.UseKestrel();

builder.Services.AddControllers();
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
    var useAspire = hostBuilder.Configuration.GetValue<bool>("USE_ASPIRE");

    clientBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "csharpwars-orleans";
        options.ServiceId = "csharpwars-orleans";
    });

    if (useAspire)
    {
        clientBuilder.UseLocalhostClustering(
            gatewayPort: 30001,
            clusterId: "csharpwars-orleans",
            serviceId: "csharpwars-orleans");
    }
    else
    {
        var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        clientBuilder.UseAzureStorageClustering(options =>
        {
            options.TableServiceClient = new TableServiceClient(azureStorageConnectionString);
        });
    }
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
        var fileName = context.File.Name;
        if (fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
        {
            context.Context.Response.Headers["Content-Encoding"] = "gzip";

            if (fileName.EndsWith(".js.gz", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.ContentType = "application/javascript";
            }
            else if (fileName.EndsWith(".wasm.gz", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.ContentType = "application/wasm";
            }
            else if (fileName.EndsWith(".data.gz", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.ContentType = "application/octet-stream";
            }
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

app.MapDefaultEndpoints();

app.Run();