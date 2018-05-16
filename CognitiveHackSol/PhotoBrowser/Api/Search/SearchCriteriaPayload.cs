using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Api.Search
{
    public class SearchCriteriaPayload
    {
        public string[] Tags { get; set; }

        public string[] Categories { get; set; }

        public string[] Captions { get; set; }
    }
}