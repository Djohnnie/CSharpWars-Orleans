using CSharpWars.Orleans.Grains;
using CSharpWars.WebApi;
using Microsoft.AspNetCore.Mvc;
using Orleans;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());
builder.Services.AddSingleton(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);

var app = builder.Build();
app.UsePathBase("/api");

app.MapGet("/", async ([FromServices] IClusterClient _client) =>
{
    var grain = _client.GetGrain<IArenaGrain>(Guid.Empty);
    await grain.IncreaseCounter();
    return await grain.GetMessage();
});

app.Run();