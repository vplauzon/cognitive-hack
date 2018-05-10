using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Payloads
{
    public class StatusOutputPayload
    {
        public string Status { get; set; }

        public int PhotoCount { get; set; }
    }
}