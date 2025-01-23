namespace ZeroAdBrowser.TrackersProvider.Configuration;

internal class RedisCacheConfig
{
    public string ConnectionString { get; set; }

    public string InstanceName { get; set; }

    public string CacheKey { get; set; }

    public int CacheDurationInDays { get; set; }
}
