using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using KX.StorageService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace KX.StorageService.iTests
{
    public class DataControllerTests : IClassFixture<WebApplicationFactory<Startup>>
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

        private readonly WebApplicationFactory<Startup> factory;
        private readonly ITestOutputHelper testOutputWriter;

        public DataControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper testOutputWriter)
        {
            this.testOutputWriter = testOutputWriter;
            this.factory = factory.WithWebHostBuilder((webHostBuilder) =>
            {
                webHostBuilder.ConfigureServices(services =>
                {
                    //Provide test data 
                    services.AddSingleton<IDataProvider, InMemoryDataProvider>(sp =>
                        new InMemoryDataProvider(testData));
                });
            });
        }

        [Fact]
        public async Task Data_Returns_All_Partitions()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/data");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo("[\"testPartition1\",\"testPartition2\"]");
        }

        [Fact]
        public async Task Data_Partition_Returns_All_Identifiers()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/data/testPartition1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo("[\"testIdentifier1\",\"testIdentifier2\"]");
        }

        [Fact]
        public async Task Data_Non_Existent_Partition_Returns_HTTP404()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/data/testPartition4");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Data_Partition_And_Identifier_Returns_Data()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/data/testPartition1/testIdentifier1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo("{\"Name\":\"Foo\",\"Age\":140}");
        }

        [Fact]
        public async Task Data_Non_Existent_Identifier_Returns_HTTP404()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/data/testPartition1/testIdentifier5");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
