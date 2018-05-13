using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Models
{
    public class SearchModel
    {
        public int TotalImages { get; set; }

        public int TotalCategories { get; set; }

        public GroupModel[] Categories { get; set; }

        public int TotalCaptions { get; set; }

        public GroupModel[] Captions { get; internal set; }

        public int TotalTags { get; set; }

        public GroupModel[] Tags { get; internal set; }
    }
}
