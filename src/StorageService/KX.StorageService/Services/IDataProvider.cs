using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KX.StorageService.Services
{
    public interface IDataProvider
    {
        Task<IEnumerable<string>> GetPartitions();

        Task<bool> TryGetPartition(string partitionKey, out IReadOnlyDictionary<string, JObject> partition);

        Task<bool> TryGetData(string partitionKey, string identifier, out JObject result);
    }
}
