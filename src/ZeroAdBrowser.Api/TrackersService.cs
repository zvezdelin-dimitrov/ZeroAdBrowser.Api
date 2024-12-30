using Azure.Storage.Blobs;
using System.Net.Http.Headers;
using System.Text.Json;
using ZeroAdBrowser.Api.Configuration;
using ZeroAdBrowser.Api.Models;

internal sealed class TrackersService
{   
    private readonly IHttpClientFactory httpClientFactory;
    private readonly Config config;
    private readonly BlobServiceClient blobServiceClient;

    public TrackersService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        config = configuration.Get<Config>();
        blobServiceClient = new(config.BlobStorage.ConnectionString);
    }

    public async Task<List<TrackerResult>> GetTrackers()
    {
        var currentTrackerData = await LoadFromFile();

        try
        {
            using var client = httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, config.TrackerListUrl);

            if (!string.IsNullOrWhiteSpace(currentTrackerData.ETag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(currentTrackerData.ETag));
            }

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var parsedTrackerList = await JsonSerializer.DeserializeAsync<TrackerList>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await SaveToFile(new TrackerData { ETag = response.Headers.ETag?.Tag, TrackerList = parsedTrackerList });

            return ConvertToResult(parsedTrackerList);
        }
        catch
        {
        }

        return ConvertToResult(currentTrackerData.TrackerList);
    }    

    private async Task<TrackerData> LoadFromFile()
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(config.BlobStorage.ContainerName);
            var blobClient = containerClient.GetBlobClient(config.BlobStorage.BlobName);

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using var stream = response.Value.Content;
                return await JsonSerializer.DeserializeAsync<TrackerData>(stream);
            }
        }
        catch
        {
        }        

        return new TrackerData();
    }

    private async Task SaveToFile(TrackerData trackerData)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(config.BlobStorage.ContainerName);
            await containerClient.CreateIfNotExistsAsync();

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, trackerData);

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
