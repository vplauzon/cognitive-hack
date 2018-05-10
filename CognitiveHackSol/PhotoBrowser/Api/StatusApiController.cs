using Microsoft.AspNetCore.Mvc;
using PhotoBrowser.Api.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Api
{
    [Route("/api/status")]
    public class StatusApiController : Controller
    {
        [HttpPost]
        public StatusOutputPayload Index([FromBody]StatusInputPayload input)
        {
            return new StatusOutputPayload
            {
                Status="Hi",
                DocumentCount = 42
            };
        }
    }
}