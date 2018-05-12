using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Models
{
    public class SearchModel
    {
        public GroupModel[] Categories { get; set; }
        public GroupModel[] Captions { get; internal set; }
        public GroupModel[] Tags { get; internal set; }
    }
}
