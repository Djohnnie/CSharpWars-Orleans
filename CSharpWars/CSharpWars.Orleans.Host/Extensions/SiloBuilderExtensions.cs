using Orleans.Hosting;

namespace CSharpWars.Orleans.Host.Extensions;

public static class SiloBuilderExtensions
{
    public static ISiloBuilder AddAzureBlobGrainStorage(this ISiloBuilder siloBuilder, string name, string connectionString)
    {
        return siloBuilder.AddAzureBlobGrainStorage(
            name, configureOptions: options =>
            {
                options.UseJson = true;
                options.ConfigureBlobServiceClient(connectionString);
            });
    }
}