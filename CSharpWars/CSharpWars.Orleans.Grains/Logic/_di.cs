using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Orleans.Grains.Logic;

public static class ServiceCollectionExtensions
{
    public static void AddGrainLogic(this IServiceCollection services)
    {
        services.AddScoped<IProcessorLogic, ProcessorLogic>();
        services.AddScoped<IPreprocessingLogic, PreprocessingLogic>();
        services.AddScoped<IProcessingLogic, ProcessingLogic>();
        services.AddScoped<IPostprocessingLogic, PostprocessingLogic>();
    }
}