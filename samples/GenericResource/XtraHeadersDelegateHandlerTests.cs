using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Squadron.Samples.Generic
{
    public class XtraHeadersDelegateHandlerTests
        : IClassFixture<GenericContainerResource<HttpBinWebServerOptions>>
    {
        private readonly GenericContainerResource<HttpBinWebServerOptions> _genericContainer;

        public XtraHeadersDelegateHandlerTests(
            GenericContainerResource<HttpBinWebServerOptions> genericContainer)
        {
            _genericContainer = genericContainer;
        }

        [Fact]
        public async Task Send_WithHandler_HeaderEchoed()
        {
            //arrange
            var handler = new XtraHeadersDelegateHandler();
            var client = new HttpClient(handler);
            client.BaseAddress = _genericContainer.GetContainerUri();

            //act
            HttpResponseMessage response = await client.GetAsync("headers");

            //assert
            string json = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> echoedHeaders = GetHeadersFromJson(json);
            echoedHeaders.Should().ContainKey("X-Fancy");
        }

        private Dictionary<string, object> GetHeadersFromJson(string json)
        {
            var jObject = JObject.Parse(json);
            Dictionary<string, object> headers =
                JsonSerializer.Deserialize<Dictionary<string, object>>(
                    jObject.SelectToken("headers").ToString());
            return headers;
        }
    }


    public class HttpBinWebServerOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("http-bin")
                .InternalPort(80)
                .Image("kennethreitz/httpbin");

            ConfigureHttpStatusChecker("status/200");
        }
    }
}
