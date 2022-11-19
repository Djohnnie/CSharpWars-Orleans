using CSharpWars.Common.Helpers;
using CSharpWars.Mappers;
using CSharpWars.Orleans.Common;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Extensions;
using CSharpWars.WebApi.Helpers;
using CSharpWars.WebApi.Managers;
using CSharpWars.WebApi.Middleware;
using CSharpWars.WebApi.Security;
using Orleans.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAutoMapper(typeof(StatusMapperProfile));

builder.Services.AddOrleansHelpers();

builder.Services.AddScoped<IPlayerContext, PlayerContext>();

builder.Services.AddCommonHelpers();
builder.Services.AddHelpers();
builder.Services.AddManagers();

builder.Host.UseOrleans((hostBuilder, siloBuilder) =>
{
    var azureStorageConnectionString = hostBuilder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

#if DEBUG
    //siloBuilder.UsePerfCounterEnvironmentStatistics();
    siloBuilder.UseLocalhostClustering(siloPort: 11113, gatewayPort: 30002, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "csharpwars-orleans-host", clusterId: "csharpwars-orleans-host");
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

app.UseMiddleware<JwtMiddleware>();

app.UsePathBase("/api");

app.MapGet("/", async (IApiHelper<IStatusManager> helper) =>
{
    return await helper.Execute(m => m.GetStatus());
});

app.MapPost("/players", async (LoginRequest request, IApiHelper<IPlayerManager> helper) =>
{
    return await helper.Execute(m => m.Login(request));
});

app.MapGet("/arena/{name}", async (string name, IApiHelper<IArenaManager> helper) =>
{
    var request = new GetArenaRequest { Name = name };
    return await helper.Execute(m => m.GetArena(request));
});

app.MapGet("/arena/{name}/bots", async (string name, IApiHelper<IBotManager> helper) =>
{
    var request = new GetAllActiveBotsRequest { ArenaName = name };
    return await helper.Execute(m => m.GetAllActiveBots(request));
});

app.MapGet("/arena/{name}/messages", async (string name, IApiHelper<IMessagesManager> helper) =>
{
    var request = new GetAllMessagesRequest { ArenaName = name };
    return await helper.Execute(m => m.GetAllMessages(request));
});

app.MapGet("/arena/{name}/moves", async (string name, IApiHelper<IMovesManager> helper) =>
{
    var request = new GetAllMovesRequest { ArenaName = name };
    return await helper.Execute(m => m.GetMoves(request));
});

app.MapAuthorizedPost("/arena/{name}/bots", async (string name, CreateBotRequest request, IPlayerContext playerContext, IApiHelper<IBotManager> helper) =>
{
    var finalRequest = new CreateBotRequest
    {
        ArenaName = name,
        PlayerName = playerContext.PlayerName,
        BotName = request.BotName,
        MaximumHealth = request.MaximumHealth,
        MaximumStamina = request.MaximumStamina,
        Script = request.Script,
    };

    return await helper.Execute(m => m.CreateBot(finalRequest));
});

app.MapAdminDelete("/players", async (IApiHelper<IPlayerManager> helper) =>
{
    await helper.Execute(m => m.DeleteAllPlayers());
});

app.MapAdminDelete("/arena/{name}", async (string name, IApiHelper<IArenaManager> helper) =>
{
    await helper.Execute(m => m.DeleteArena(name));
});

app.Run();