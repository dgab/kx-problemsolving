using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KX.StorageService.uTests
{
    public class InMemoryDataProviderTests
    {
        private static Dictionary<string, IReadOnlyDictionary<string, JObject>> testData =
            new Dictionary<string, IReadOnlyDictionary<string, JObject>>()
            {
                { "testPartition1", new Dictionary<string, JObject>()
                    {
                        { "testIdentifier1", JObject.Parse("{\"Name\":\"Foo\", \"Age\":140}")},
                        { "testIdentifier2", JObject.Parse("{\"Name\":\"Melinda\", \"Age\":56}")}
                    }
                },
                { "testPartition2", new Dictionary<string, JObject>()
                    {
                        { "testIdentifier1", JObject.Parse("{\"Name\":\"John\", \"Age\":30}")},
                        { "testIdentifier2", JObject.Parse("{\"Name\":\"Johnny\", \"Age\":31}")},
                        { "testIdentifier3", JObject.Parse("{\"Name\":\"Albert\", \"Age\":1}")}
                    }
                }
            };

        [Fact]
        public async Task GetPartitions_Returns_All_Partitions()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            (await dataProvider.GetPartitions())
                .Should().BeEquivalentTo(new[] { "testPartition1", "testPartition2" });
        }

        [Fact]
        public async Task TryGetPartition_Returns_True_If_Partition_Exists()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            var result = await dataProvider.TryGetPartition("testPartition1", out var partition);

            result.Should().BeTrue();
            partition.Should().BeEquivalentTo(new Dictionary<string, JObject>(){
                        { "testIdentifier1", JObject.Parse("{\"Name\":\"Foo\", \"Age\":140}")},
                        { "testIdentifier2", JObject.Parse("{\"Name\":\"Melinda\", \"Age\":56}")}
                    });
        }

        [Fact]
        public async Task TryGetPartition_Returns_False_If_Partition__Does_Not_Exists()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            var result = await dataProvider.TryGetPartition("testPartition3", out var partition);

            result.Should().BeFalse();
            partition.Should().BeNull();
        }

        [Fact]
        public async Task TryGetData_Returns_True_If_Partition_And_Id_Exists()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            var result = await dataProvider.TryGetData("testPartition1", "testIdentifier1", out var data);

            result.Should().BeTrue();
            data.Should().BeEquivalentTo(JObject.Parse("{\"Name\":\"Foo\", \"Age\":140}"));
        }

        [Fact]
        public async Task TryGetData_Returns_False_If_Partition_Does_Not_Exists()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            var result = await dataProvider.TryGetData("testPartition3", "testIdentifier1", out var data);

            result.Should().BeFalse();
            data.Should().BeNull();
        }

        [Fact]
        public async Task TryGetData_Returns_False_If_Identifier_Does_Not_Exists()
        {
            InMemoryDataProvider dataProvider = new InMemoryDataProvider(testData);

            var result = await dataProvider.TryGetData("testPartition1", "testIdentifier4", out var data);

            result.Should().BeFalse();
            data.Should().BeNull();
        }
    }
}
