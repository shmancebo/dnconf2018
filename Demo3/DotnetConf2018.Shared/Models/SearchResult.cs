using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetConf2018.Services.Models
{
    public class SearchResult<T> where T : class
    {
        public long? Count { get; set; }
        public IEnumerable<T> Collection { get; set; }
        public FacetResults Facet { get; set; }
        public SearchContinuationToken ContinuationToken { get; set; }

    }
}
