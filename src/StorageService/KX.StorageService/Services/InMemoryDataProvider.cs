using System.Collections.Generic;
using System.Threading.Tasks;
using KX.StorageService.Services;
using Newtonsoft.Json.Linq;

namespace KX.StorageService
{
    public class InMemoryDataProvider : IDataProvider
    {
        //Async methods does not make sense with in memory provider, but if we are thinking about the interface,
        //probably any other data source would need to be async.

        private Dictionary<string, IReadOnlyDictionary<string, JObject>> data;

        public InMemoryDataProvider(Dictionary<string, IReadOnlyDictionary<string, JObject>> data)
        {
            this.data = data;
        }

        public InMemoryDataProvider()
            :this(new Dictionary<string, IReadOnlyDictionary<string, JObject>>()
            {
                { "partition1", new Dictionary<string, JObject>() 
                    {
                        { "identifier1", JObject.Parse("{\"Name\":\"Foo\", \"Age\":140}")},
                        { "identifier2", JObject.Parse("{\"Name\":\"Melinda\", \"Age\":56}")}
                    } 
                },
                { "partition2", new Dictionary<string, JObject>()
                    {
                        { "identifier1", JObject.Parse("{\"Name\":\"John\", \"Age\":30}")},
                        { "identifier2", JObject.Parse("{\"Name\":\"Johnny\", \"Age\":31}")},
                        { "identifier3", JObject.Parse("{\"Name\":\"Albert\", \"Age\":1}")}
                    }
                }
            })
        {
        }

        public Task<IEnumerable<string>> GetPartitions()
        {
            IEnumerable<string> result = data.Keys;
            return Task.FromResult(result);
        }

        public Task<bool> TryGetPartition(string partitionKey, out IReadOnlyDictionary<string, JObject> partition)
        {
            if (!data.TryGetValue(partitionKey, out partition))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> TryGetData(string partitionKey, string identifier, out JObject result)
        {
            result = null;

            if (!data.TryGetValue(partitionKey, out var partition))
            {
                return Task.FromResult(false);
            }

            if (!partition.TryGetValue(identifier, out result))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
