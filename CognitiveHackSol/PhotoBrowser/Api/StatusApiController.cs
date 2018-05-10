using CosmosIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Api.Payloads;
using PhotoBrowser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace PhotoBrowser.Api
{
    [Route("/api/status")]
    public class StatusApiController : Controller
    {
        private readonly ConnectionConfiguration _apiConfiguration;

        public StatusApiController(IOptions<ConnectionConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        [HttpPost]
        public StatusOutputPayload Index([FromBody]StatusInputPayload input)
        {
            var cosmosService = new CosmosService(
                _apiConfiguration.CosmosDbEndpoint,
                _apiConfiguration.CosmosDbKey,
                SessionFilterAttribute.GetSessionId(HttpContext));

            return new StatusOutputPayload
            {
                Status="Hi",
                PhotoCount = 42
            };
        }
    }
}