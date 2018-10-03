using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compartimos.Indexation.Services;
using DotnetConf2018.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DotnetConf2018.Presentation.Controllers
{
    [Route("api/Search")]
    public class SearchController : Controller
    {
        [HttpGet]
        public string SearchQuery(string index,string query)
        {
          var client = new AzureSearchService("dotnetconfsearch", "162ED4F1F498A366C066F1A1B1430F5D");
          var result = client.RunQuery<AzureSearchModel>(index, new Microsoft.Azure.Search.Models.SearchParameters(), query, null).Result;
          return JsonConvert.SerializeObject(result.Collection);
        }
    }
}
