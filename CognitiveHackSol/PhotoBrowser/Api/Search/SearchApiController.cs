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
    [SessionFilter]
    [Route("/api/search")]
    public class SearchApiController : Controller
    {
        private const int MAX_IMAGE_COUNT = 50;

        private readonly ConnectionConfiguration _apiConfiguration;

        public SearchApiController(IOptions<ConnectionConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        public async Task<SearchOutputPayload> Index([FromBody]SearchCriteriaPayload criteria)
        {
            var cosmosService = GetCosmosService();
            //  Ask both in parallel
            var data = await cosmosService.SearchAsync(
                MAX_IMAGE_COUNT,
                criteria.Tags,
                criteria.Categories,
                criteria.Captions);
            var images = from d in data.Images
                         select new ImagePayload
                         {
                             ThumbnailUrl = d.ThumbnailUrl,
                             Captions = from c in d.Captions
                                        orderby c.Confidence descending
                                        select c.Text,
                             Categories = from c in d.Categories
                                          orderby c.Score descending
                                          select c.Name,
                             Tags = from t in d.Tags
                                    orderby t.Confidence descending
                                    select t.Name
                         };
            var output = new SearchOutputPayload
            {
                Images = images,
                TotalAvailable = data.TotalAvailable
            };

            return output;
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