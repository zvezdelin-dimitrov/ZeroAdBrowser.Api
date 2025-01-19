var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

var config = builder.Configuration.Get<ZeroAdBrowser.Api.Configuration.Config>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = config.RedisCache.ConnectionString;
    options.InstanceName = config.RedisCache.InstanceName;
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<TrackersService>();

var app = builder.Build();

app.MapGet("/trackers", (TrackersService trackersService) => trackersService.GetTrackers());

app.Run();
