namespace ZeroAdBrowser.Api.Configuration;

public class Config
{
    public BlobStorageConfig BlobStorage { get; set; }

    public RedisCacheConfig RedisCache { get; set; }

    public string TrackerListUrl { get; set; }
}
