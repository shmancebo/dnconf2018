using DotnetConf2018.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetConf2018.Portal.Models
{
    public class SearchViewModel
    {
        public IEnumerable<AzureSearchModel> SearchResult { get; set; }
        public string Query { get; set; }
    }
}
