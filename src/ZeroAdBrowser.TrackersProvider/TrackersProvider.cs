using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ZeroAdBrowser.TrackersProvider.Configuration;
using ZeroAdBrowser.TrackersProvider.Models;

internal class TrackersProvider(IHttpClientFactory httpClientFactory, IDistributedCache cache, BlobServiceClient blobServiceClient, IOptions<JsonSerializerOptions> jsonSerializerOptions, IConfiguration configuration) : ITrackersProvider
{   
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
    private readonly IDistributedCache cache = cache;    
    private readonly BlobServiceClient blobServiceClient = blobServiceClient;
    private readonly JsonSerializerOptions jsonSerializerOptions = jsonSerializerOptions.Value;
    private readonly Config config = configuration.Get<Config>();

    public async Task<List<TrackerResult>> GetTrackers()
    {
        var cachedResult = await LoadFromCache();

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var currentTrackerData = await LoadFromBlob();

        var newTrackerData = await LoadFromUrl(currentTrackerData.ETag);

        if (newTrackerData is not null)
        {
            await SaveToBlob(newTrackerData);

            currentTrackerData = newTrackerData;
        }

        var result = ConvertToResult(currentTrackerData.TrackerList);

        await SaveToCache(result);

        return result;
    }

    private async Task<TrackerData> LoadFromUrl(string currentTag)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, config.TrackerListUrl);

            if (!string.IsNullOrWhiteSpace(currentTag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(currentTag));
            }

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var trackerList = await JsonSerializer.DeserializeAsync<TrackerList>(stream, jsonSerializerOptions);

            return new TrackerData { ETag = response.Headers.ETag?.Tag, TrackerList = trackerList };
        }
        catch
        {
        }

        return null;
    }

    private async Task<List<TrackerResult>> LoadFromCache()
    {
        try
        {
            var cachedData = await cache.GetStringAsync(config.RedisCache.CacheKey);
            if (cachedData is not null)
            {
                return JsonSerializer.Deserialize<List<TrackerResult>>(cachedData, jsonSerializerOptions);
            }
        }
        catch
        {
        }

        return null;
    }

    private async Task SaveToCache(List<TrackerResult> trackers)
    {
        try
        {
            await cache.SetStringAsync(
                config.RedisCache.CacheKey,
                JsonSerializer.Serialize(trackers, jsonSerializerOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(config.RedisCache.CacheDurationInDays) });
        }
        catch
        {
        }
    }

    private async Task<TrackerData> LoadFromBlob()
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(config.BlobStorage.ContainerName);
            var blobClient = containerClient.GetBlobClient(config.BlobStorage.BlobName);

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using var stream = response.Value.Content;
                return await JsonSerializer.DeserializeAsync<TrackerData>(stream, jsonSerializerOptions);
            }
        }
        catch
        {
        }        

        return new TrackerData();
    }

    private async Task SaveToBlob(TrackerData trackerData)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(config.BlobStorage.ContainerName);
            await containerClient.CreateIfNotExistsAsync();

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, trackerData, jsonSerializerOptions);

            stream.Position = 0;

            var blobClient = containerClient.GetBlobClient(config.BlobStorage.BlobName);
            await blobClient.UploadAsync(stream, true);
        }
        catch
        {
        }        
    }

    private static List<TrackerResult> ConvertToResult(TrackerList trackerList)
        => trackerList.Trackers
                      .Select(x => x.Value)
                      .Select(tracker => new TrackerResult
                      {
                          Domain = tracker.Domain,
                          Default = tracker.Default,
                          Rules = tracker.Rules?.Select(rule => new RuleDefinitionResult { Rule = rule.Rule, Action = rule.Action }).ToList()
                      })
                      .ToList();
}
