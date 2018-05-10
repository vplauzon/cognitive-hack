using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Configuration;
using PhotoBrowser.Models;
using Newtonsoft.Json;

namespace PhotoBrowser.Controllers
{
    public class SessionController : Controller
    {
        private readonly ApiConfiguration _apiConfiguration;

        public SessionController(IOptions<ApiConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
        }

        public IActionResult Index()
        {
            HttpContext.Response.Cookies.Delete(SessionFilterAttribute.SESSION_COOKIE);

            return View(new SessionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(SessionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SessionID))
            {
                var ingestPhotosApiUrl = _apiConfiguration.IngestPhotosApiUrl;

                if (string.IsNullOrWhiteSpace(ingestPhotosApiUrl))
                {
                    throw new ApplicationException("No API URL found");
                }

                var apiRequest = WebRequest.Create(ingestPhotosApiUrl);

                apiRequest.Method = "POST";
                using (var apiRequestStream = await apiRequest.GetRequestStreamAsync())
                {
                    using (var response = await apiRequest.GetResponseAsync())
                    {
                        var webResponse = response as HttpWebResponse;

                        if (webResponse.StatusCode != HttpStatusCode.Accepted)
                        {
                            throw new ApplicationException("API error");
                        }

                        var payload = await GetResponsePayloadAsync<IDictionary<string, string>>(webResponse);
                        var sessionID = payload.Values.First();

                        HttpContext.Response.Cookies.Append(SessionFilterAttribute.SESSION_COOKIE, sessionID);
                    }
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append(SessionFilterAttribute.SESSION_COOKIE, model.SessionID);
            }

            return RedirectToAction(null, "Home");
        }

        private async Task<T> GetResponsePayloadAsync<T>(HttpWebResponse response)
        {
            var responseStream = response.GetResponseStream();
            var memoryStream = new MemoryStream();
            var serializer = new JsonSerializer();

            await responseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var reader = new StreamReader(memoryStream);
            var payload = serializer.Deserialize<T>(new JsonTextReader(reader));

            return payload;
        }
    }
}