using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Status
{
    public class StatusPayload
    {
        public string Status { get; set; }

        public int ImageCount { get; set; }
    }
}