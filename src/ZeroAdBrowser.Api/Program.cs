using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Azure;
using System.Text.Json.Serialization;
using ZeroAdBrowser.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var config = builder.Configuration.Get<Config>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = config.RedisCache.ConnectionString;
    options.InstanceName = config.RedisCache.InstanceName;
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(config.BlobStorage.ConnectionString);
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<TrackersService>();

var app = builder.Build();

app.MapGet("/trackers", (TrackersService trackersService) => trackersService.GetTrackers());

app.Run();
