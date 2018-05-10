using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace PhotoBrowser.Configuration
{
    public class ConnectionConfiguration
    {
        public string IngestPhotosApiUrl { get; set; }

        public Uri CosmosDbEndpoint { get; set; }

        public string CosmosDbKey { get; set; }
    }
}