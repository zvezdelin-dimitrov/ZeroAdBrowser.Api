var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromDays(5)));
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<TrackersService>();

var app = builder.Build();

app.UseOutputCache();

app.MapGet("/trackers", (TrackersService trackersService) => trackersService.GetTrackers());

app.Run();
