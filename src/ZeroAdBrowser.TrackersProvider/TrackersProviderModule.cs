using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZeroAdBrowser.TrackersProvider.Configuration;

public static class TrackersProviderModule
{
    public static IServiceCollection AddTrackersProvider(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        configurationManager.SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("trackersprovider.appsettings.json")
                            .AddEnvironmentVariables()
                            .AddUserSecrets<Config>();

        var config = configurationManager.Get<Config>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.RedisCache.ConnectionString;
            options.InstanceName = config.RedisCache.InstanceName;
        });

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(config.BlobStorage.ConnectionString);
        });

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
        });

        services.AddHttpClient();

        services.AddSingleton<ITrackersProvider, TrackersProvider>();

        return services;
    }
}
