using System.Collections.Generic;

namespace PhotoBrowser.Api.Search
{
    public class SearchOutputPayload
    {
        public int TotalAvailable { get; set; }

        public IEnumerable<ImagePayload> Images { get; set; }
    }
}