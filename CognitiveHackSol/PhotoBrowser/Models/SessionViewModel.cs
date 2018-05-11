using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoBrowser.Models
{
    public class SessionViewModel
    {
        public string Container { get; set; } = "test";

        public string SasToken { get; set; } = "?st=2018-05-07T14%3A24%3A18Z&se=2018-07-02T14%3A24%3A00Z&sp=r&sv=2017-04-17&sr=c&sig=AkNUwgkRkIawtmKz6g3teT7oPX0SmQa9EKGHaMaAz8k%3D";

        public string SessionId { get; set; }
    }
}