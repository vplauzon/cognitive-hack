using CosmosIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Search
{
    [Route("/api/search")]
    public class SearchApiController : Controller
    {
        private const int IMAGE_COUNT = 30;

        private readonly ConnectionConfiguration _apiConfiguration;

        public SearchApiController(IOptions<ConnectionConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        public async Task<ImagePayload[]> Index([FromQuery]string sessionId)
        {
            var cosmosService = GetCosmosService();
            //  Ask both in parallel
            var data = await cosmosService.SearchNoCriteriaAsync(IMAGE_COUNT);
            var images = from d in data
                         select new ImagePayload
                         {
                             ThumbnailUrl = d.ThumbnailUrl,
                             Captions = from c in d.Captions
                                        orderby c.Confidence descending
                                        select c.Text
                         };

            return images.ToArray();
        }

        private CosmosService GetCosmosService()
        {
            return new CosmosService(
                _apiConfiguration.CosmosDbEndpoint,
                _apiConfiguration.CosmosDbKey,
                SessionFilterAttribute.GetSessionId(HttpContext));
        }
    }
}