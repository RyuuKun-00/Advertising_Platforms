using AdvertisingPlatforms.Tests.Integration_Tests.CustomWebApplication;
using AdvertisingPlatforms.Tests.TestResources.Models;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AdvertisingPlatforms.Tests.Integration_Tests.AdvertisingPlatformsController
{
    [Collection("CommonClientWebApplication")]
    [TestCaseOrderer(
        ordererTypeName: "AdvertisingPlatforms.Tests.ResourcesTest.Orderers",
        ordererAssemblyName: "AdvertisingPlatforms.Tests.Integration_Tests.AdvertisingPlatformsController")]
    public partial class Tests_AdvertisingPlatformsController
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly AppParameters_Test _parameters;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public Tests_AdvertisingPlatformsController(CustomApiFixture fixture, ITestOutputHelper output)
        {
            _factory = fixture.Factory;
            _parameters = fixture.Parameters;
            _client = fixture.Client;
            _output = output;
        }
    }
}
