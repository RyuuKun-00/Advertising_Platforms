using AdvertisingPlatforms.Tests.TestResources.Models;
using System.Net;
using Xunit;

namespace AdvertisingPlatforms.Tests.Integration_Tests.AdvertisingPlatformsController
{
    public partial class Tests_AdvertisingPlatformsController
    {
        [Fact]
        public async Task Test_01_GET_GetParameterWebApplication()
        {
            //_output.WriteLine($"GetParameters: {DateTime.Now.Ticks}");

            /////////////
            // Arrange //
            /////////////


            /////////
            // Act //
            /////////
            var response = await _client.GetAsync("/api/advertising_platforms/application_parameters");
            var appParameters = await response.Content.ReadFromJsonAsync<AppParameters_Test>();
            ////////////
            // Assert //
            ////////////

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_parameters, appParameters);
        }

    }
}
