using System.Collections.Generic;

namespace PhotoBrowser.Api.Search
{
    public class ImagePayload
    {
        public string ThumbnailUrl { get; set; }

        public IEnumerable<string> Captions { get; set; }
}
}