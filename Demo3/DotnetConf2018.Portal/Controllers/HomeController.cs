using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetConf2018.Portal.Models;
using System.Net.Http;
using Newtonsoft.Json;
using DotnetConf2018.Services.Models;

namespace DotnetConf2018.Portal.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            if (model.Query != null)
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync($"https://dotnetapisearch.azurewebsites.net/api/search?index=csindex&query={model.Query}");
                var result = await response.Content.ReadAsStringAsync();
                model.SearchResult = JsonConvert.DeserializeObject<List<DotnetConf2018.Services.Models.AzureSearchModel>>(result);
                return View(model);
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
