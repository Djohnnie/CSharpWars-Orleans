using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IScriptGrain : IGrainWithGuidKey
{
    Task SetScript(string script);
    Task<BotProperties> Process(BotProperties botProperties);
    Task DeleteScript();
}