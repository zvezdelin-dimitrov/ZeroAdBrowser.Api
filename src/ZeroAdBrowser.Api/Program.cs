using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTrackersProvider(builder.Configuration);

var app = builder.Build();

app.MapGet("/trackers", async (ITrackersProvider trackersProvider, IOptions<JsonSerializerOptions> jsonSerializerOptions) =>
{
    var trackers = await trackersProvider.GetTrackers();
    return Results.Json(trackers, jsonSerializerOptions.Value);
});

app.Run();
