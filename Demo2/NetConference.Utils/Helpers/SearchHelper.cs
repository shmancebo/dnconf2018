using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using System.Threading.Tasks;

namespace NetConference.Utils.Helpers
{
    public class SearchHelper
    {
        public async Task UpdateIndexerAsync(TraceWriter log)
        {
            var indexerName = "edificiosindexer";
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string adminApiKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];

            var serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            if(!await serviceClient.Indexers.ExistsAsync(indexerName))
            {
                var buildingIndexer = new Indexer(indexerName, "dotnetconferenceindexer", "edificios");
                log.Info($"El indexer {indexerName} no existe, se crea");

                var res = await serviceClient.Indexers.CreateAsync(buildingIndexer);
                log.Info($"Indexer {indexerName} creado");
            }

            await serviceClient.Indexers.RunAsync(indexerName);
            log.Info($"Indexer {indexerName} lanzado");
        }
    }
}
