using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Scripting;

public static class ServiceCollectionExtensions
{
    public static void AddScripting(this IServiceCollection services)
    {
        services.AddSingleton<IScriptCompiler, ScriptCompiler>();
    }
}