using CosmosIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Group
{
    [SessionFilter]
    [Route("/api/group")]
    public class GroupApiController : Controller
    {
        private readonly ConnectionConfiguration _apiConfiguration;

        public GroupApiController(IOptions<ConnectionConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        [Route("captions")]
        public async Task<GroupPayload[]> Captions()
        {
            var cosmosService = GetCosmosService();
            //  Ask both in parallel
            var data = await cosmosService.GetAllCaptionsAsync();
            var captions = from g in data
                           orderby g.Name
                           select new GroupPayload
                           {
                               Name = g.Name,
                               Count = g.Count
                           };

            return captions.ToArray();
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