namespace ZeroAdBrowser.Api.Configuration;

public class RedisCacheConfig
{
    public string ConnectionString { get; set; }

    public string InstanceName { get; set; }

    public string CacheKey { get; set; }
}
