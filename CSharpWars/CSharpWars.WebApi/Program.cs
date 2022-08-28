using CSharpWars.Common.Helpers;
using CSharpWars.Mappers;
using CSharpWars.Orleans.Common;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Extensions;
using CSharpWars.WebApi.Helpers;
using CSharpWars.WebApi.Managers;
using CSharpWars.WebApi.Middleware;
using CSharpWars.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAutoMapper(typeof(StatusMapperProfile));

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());
builder.Services.AddSingleton(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
builder.Services.AddOrleansHelpers();

builder.Services.AddScoped<IPlayerContext, PlayerContext>();

builder.Services.AddCommonHelpers();
builder.Services.AddHelpers();
builder.Services.AddManagers();

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
    var request = new GetArenaRequest(name);
    return await helper.Execute(m => m.GetArena(request));
});

app.MapGet("/arena/{name}/bots", async (string name, IApiHelper<IBotManager> helper) =>
{
    var request = new GetAllActiveBotsRequest(name);
    return await helper.Execute(m => m.GetAllActiveBots(request));
});

app.MapGet("/arena/{name}/messages", async (string name, IApiHelper<IMessagesManager> helper) =>
{
    var request = new GetAllMessagesRequest(name);
    return await helper.Execute(m => m.GetAllMessages(request));
});

app.MapGet("/arena/{name}/moves", async (string name, IApiHelper<IMovesManager> helper) =>
{
    var request = new GetAllMovesRequest(name);
    return await helper.Execute(m => m.GetMoves(request));
});

app.MapAuthorizedPost("/arena/{name}/bots", async (string name, CreateBotRequest request, IPlayerContext playerContext, IApiHelper<IBotManager> helper) =>
{
    var finalRequest = request with { ArenaName = name, PlayerName = playerContext.PlayerName };
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

#if DEBUG
    await Task.Delay(30000);
#endif

app.Run();