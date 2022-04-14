using System;
using System.Collections.Generic;

namespace KX.StatusService.Services
{
    public class ContainerStatus
    {
        public string Id { get; init; }
        public string Name { get; init; }

        public string State { get; init; }

        public string Status { get; init; }

        public IDictionary<string, string> Labels { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
