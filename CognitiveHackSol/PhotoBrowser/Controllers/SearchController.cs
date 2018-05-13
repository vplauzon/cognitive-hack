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
            var totalImagesTask = cosmosService.GetImageCountAsync();
            var categoriesTask = cosmosService.GetAllCategoriesAsync();
            var tagsTask = cosmosService.GetAllTagsAsync();
            var captionsTask = cosmosService.GetAllCaptionsAsync();

            //  Join
            await Task.WhenAll(totalImagesTask, categoriesTask, tagsTask, captionsTask);

            var totalImages = totalImagesTask.Result;
            var categories = categoriesTask.Result;
            var tags = tagsTask.Result;
            var captions = captionsTask.Result;
            var model = new SearchModel
            {
                TotalImages = totalImages,
                TotalCategories = categories.Sum(g => g.Count),
                Categories = DataToModel(categories),
                TotalTags = tags.Sum(g => g.Count),
                Tags = DataToModel(tags),
                TotalCaptions = captions.Sum(g => g.Count),
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