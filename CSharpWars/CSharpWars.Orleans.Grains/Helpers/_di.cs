using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Orleans.Grains.Helpers;

public static class ServiceCollectionExtensions
{
    public static void AddGrainHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGrainFactoryHelper<>), typeof(GrainFactoryHelper<>));
        services.AddScoped(typeof(IGrainFactoryHelperWithGuidKey<>), typeof(GrainFactoryHelperWithGuidKey<>));
        services.AddScoped(typeof(IGrainFactoryHelperWithStringKey<>), typeof(GrainFactoryHelperWithStringKey<>));
    }
}