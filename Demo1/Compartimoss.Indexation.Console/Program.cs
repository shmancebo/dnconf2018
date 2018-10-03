using Compartimos.Indexation.Services;
using Compartimoss.Indexation.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compartimoss.Indexation
{
    class Program
    {
        static void Main(string[] args)
        {

            string searchServiceName = "dotnetconfsearch";
            string adminApiKey = "162ED4F1F498A366C066F1A1B1430F5D";
            //Get data
            string text = System.IO.File.ReadAllText(@"../../../Data/dataOscars.json");
            List<AzureSearchModel> data = JsonConvert.DeserializeObject<List<AzureSearchModel>>(text);


            //Create index
            var azureSearch = new AzureSearchService(searchServiceName, adminApiKey);
            var indexCreate = azureSearch.CreateIndexAsync<AzureSearchModel>("oscars", false, null).Result;

            // Group by year - One document by year

            var groupByYear = data.GroupBy(e => e.Year).ToList();

            foreach(var element in groupByYear)
            {
                var currentData = element.ToList<AzureSearchModel>();
                var uploadDocument = azureSearch.UploadDocuments<AzureSearchModel>("oscars", currentData.ToArray()).Result;
            }
        }
    }
}
