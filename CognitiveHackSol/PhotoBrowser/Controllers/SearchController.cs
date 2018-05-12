using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosIntegration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Configuration;
using PhotoBrowser.Models;

namespace PhotoBrowser.Controllers
{
    [SessionFilter]
    public class SearchController : Controller
    {
        private readonly ConnectionConfiguration _apiConfiguration;

        public SearchController(IOptions<ConnectionConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        public async Task<IActionResult> Index()
        {
            var cosmosService = GetCosmosService();
            var categories = await cosmosService.GetAllCategoriesAsync();
            var tags = await cosmosService.GetAllTagsAsync();
            var captions = await cosmosService.GetAllCaptionsAsync();
            var model = new SearchModel
            {
                Categories = DataToModel(categories),
                Tags = DataToModel(tags),
                Captions = DataToModel(captions)
            };

            return View(model);
        }

        private GroupModel[] DataToModel(GroupData[] data)
        {
            var models = from d in data
                         orderby d.Text
                         select new GroupModel
                         {
                             Text = d.Text,
                             Count = d.Count
                         };

            return models.ToArray();
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