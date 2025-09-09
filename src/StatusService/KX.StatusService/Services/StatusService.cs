using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using KX.StatusService.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace KX.StatusService.Services
{

    public class StatusService : IStatusService
    {
        const string cacheKey = "StatusService_Cache";

        private readonly StatusServiceConfig config;
        private readonly IMemoryCache memoryCache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public StatusService(StatusServiceConfig config, IMemoryCache memoryCache)
        {
            this.config = config;
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<ContainerStatus>> GetContainerStatuses(string id, string label)
        {
            string ck = $"{cacheKey}_{id ?? "null"}_{label ?? "null"}";

            //TODO: Exception handling
            if (memoryCache.TryGetValue<IEnumerable<ContainerStatus>>(ck, out var cacheEntry))
            {
                return cacheEntry;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (memoryCache.TryGetValue<IEnumerable<ContainerStatus>>(ck, out cacheEntry))
                {
                    return cacheEntry;
                }

                return await GetContainerStatusesInternal(id, label);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IEnumerable<ContainerStatus>> GetContainerStatusesInternal(string id, string appname)
        {
            using (DockerClient _dockerClient = new DockerClientConfiguration(config.DockerSockerUri).CreateClient())
            {
                IList<ContainerListResponse> response = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());

                var result = response
                    .Where(FilterById(id))
                    .Where(FilterByAppName(appname))
                    .Select(x => new ContainerStatus()
                    {
                        Id = x.ID,
                        Name = string.Join(",", x.Names),
                        State = x.State,
                        Status = x.Status,
                        CreatedAt = x.Created,
                        Labels = x.Labels.Where(x => x.Key.StartsWith("app"))?.ToDictionary(x => x.Key, y => y.Value)
                    });

                memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.UtcNow.Add(config.CacheAbsoluteExpiration)
                });

                return result;
            }
        }

        private static Func<ContainerListResponse, bool> FilterByAppName(string appname)
        {
            return x => string.IsNullOrWhiteSpace(appname) || 
                (x.Labels.TryGetValue("app.name", out string appLabel) && string.Equals(appLabel, appname, StringComparison.OrdinalIgnoreCase));
        }

        private static Func<ContainerListResponse, bool> FilterById(string id)
        {
            return x => string.IsNullOrWhiteSpace(id) || string.Equals(x.ID, id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
