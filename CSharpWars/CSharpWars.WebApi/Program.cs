using CSharpWars.Helpers;
using CSharpWars.Mappers;
using CSharpWars.WebApi;
using CSharpWars.WebApi.Contracts;
using CSharpWars.WebApi.Helpers;
using CSharpWars.WebApi.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAutoMapper(typeof(StatusMapperProfile));

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());
builder.Services.AddSingleton(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);

builder.Services.AddCommonHelpers();
builder.Services.AddHelpers();
builder.Services.AddManagers();

var app = builder.Build();
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

app.MapPost("/arena/{name}/bots", async (string name, CreateBotRequest request, IApiHelper<IBotManager> helper) =>
{
    return await helper.Execute(m => m.CreateBot(request));
});

app.Run();