namespace CosmosIntegration
{
    public class SearchImageData
    {
        public string ThumbnailUrl { get; set; }

        public CaptionData[] Captions { get; set; }

        public CategoryData[] Categories { get; set; }

        public TagData[] Tags { get; set; }
    }
}