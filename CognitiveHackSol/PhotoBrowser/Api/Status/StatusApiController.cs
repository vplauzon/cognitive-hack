﻿using CosmosIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhotoBrowser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Status
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
        public async Task<OutputPayload> Index([FromBody]InputPayload input)
        {
            var cosmosService = new CosmosService(
                _apiConfiguration.CosmosDbEndpoint,
                _apiConfiguration.CosmosDbKey,
                SessionFilterAttribute.GetSessionId(HttpContext));
            //  Ask both in parallel
            var statusTask = cosmosService.GetStatusAsync();
            var photoCountTask = cosmosService.GetImageCountAsync();

            await Task.WhenAll(statusTask, photoCountTask);

            return new OutputPayload
            {
                Status = statusTask.Result,
                ImageCount = photoCountTask.Result
            };
        }
    }
}