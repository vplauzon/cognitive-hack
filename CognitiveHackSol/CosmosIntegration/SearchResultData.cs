using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosIntegration
{
    public class SearchResultData
    {
        public int TotalAvailable { get; set; }

        public SearchImageData[] Images { get; set; }
    }
}