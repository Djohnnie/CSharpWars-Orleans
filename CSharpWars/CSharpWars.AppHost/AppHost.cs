var builder = DistributedApplication.CreateBuilder(args);

var orleansHost = builder.AddProject<Projects.CSharpWars_Orleans_Host>("orleans-host")
    .WithEnvironment("USE_ASPIRE", "true")
    .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
    .WithEnvironment("ARENA_WIDTH", "8")
    .WithEnvironment("ARENA_HEIGHT", "8")
    .WithEnvironment("DEPLOYMENT_LIMIT", "10")
    .WithEnvironment("JWT_SECRET", "local-development-jwt-secret-csharpwars-2026")
    .WithHttpEndpoint(name: "dashboard")
    .WithUrlForEndpoint("dashboard", url =>
    {
        url.Url = "/";
        url.DisplayText = "Orleans Dashboard";
    });

var validationHost = builder.AddProject<Projects.CSharpWars_Orleans_Validation_Host>("orleans-validation-host")
    .WithEnvironment("USE_ASPIRE", "true")
    .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
    .WithHttpEndpoint(name: "dashboard")
    .WithUrlForEndpoint("dashboard", url =>
    {
        url.Url = "/";
        url.DisplayText = "Orleans Dashboard";
    })
    .WaitFor(orleansHost);

var webApi = builder.AddProject<Projects.CSharpWars_WebApi>("web-api")
    .WithEnvironment("USE_ASPIRE", "true")
    .WithEnvironment("JWT_SECRET", "local-development-jwt-secret-csharpwars-2026")
    .WithEnvironment("ADMIN_KEY", "local-development-admin-key")
    .WaitFor(orleansHost)
    .WaitFor(validationHost)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "CSharpWars API";
    });

builder.AddProject<Projects.CSharpWars_Web>("web")
    .WithEnvironment("USE_ASPIRE", "true")
    .WithEnvironment("API_BASE_ADDRESS", webApi.GetEndpoint("https"))
    .WithEnvironment("ARENA_WIDTH", "8")
    .WithEnvironment("ARENA_HEIGHT", "8")
    .WithEnvironment("POINTS_LIMIT", "200")
    .WithEnvironment("ARENA_URL", "#")
    .WithEnvironment("SCRIPT_TEMPLATE_URL", "#")
    .WithEnvironment("ENABLE_CUSTOM_PLAY", "true")
    .WithEnvironment("ENABLE_TEMPLATE_PLAY", "true")
    .WithEnvironment("QUICK_PLAY", "false")
    .WaitFor(orleansHost)
    .WaitFor(validationHost)
    .WaitFor(webApi)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "CSharpWars Web App";
    });

builder.Build().Run();