using System;

namespace KX.StatusService.Configuration
{
    public class StatusServiceConfig
    {
        public Uri DockerSockerUri { get; set; }

        public TimeSpan CacheAbsoluteExpiration { get; set; }

    }
}
