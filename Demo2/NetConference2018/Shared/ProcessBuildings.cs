using Microsoft.Azure.WebJobs.Host;
using NetConference.Utils.CsvMappers;
using NetConference.Utils.Helpers;
using NetConference.Utils.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetConference2018.Shared
{
    public class ProcessBuildings
    {
        readonly CosmosHelper<CoverageBuilding> _cosmosHelper;
        public ProcessBuildings()
        {
            _cosmosHelper = new CosmosHelper<CoverageBuilding>();
        }

        public async Task ProccessByFileAsync(string fileName, string functionName, TraceWriter log)
        {
            log.Info($"{functionName} Recuperando el fichero");
            IEnumerable<CoverageBuilding> coverageBuildingList = null;
            using (var ms = BlobHelper.GetStream(fileName, "edificios"))
            {
                coverageBuildingList = NetConference.Utils.Helpers.CsvHelper.ReadCsvFileWithMapperAndHeader<CoverageBuilding, CoverageBuildingCsvMap>(ms).ToList();
            }

            await ProccessByItemListAsync(coverageBuildingList, functionName, log);
        }

        public async Task ProccessByItemListAsync(IEnumerable<CoverageBuilding> coverageBuildingList, string functionName, TraceWriter log)
        {
            log.Info($"{functionName} Registros encontrados {coverageBuildingList.Count()}");
            if (coverageBuildingList.Any())
            {
                var gescales = coverageBuildingList.Select(b => b.Gescal).ToList();

                await _cosmosHelper.InitializeAsync(log);
                log.Info($"{functionName} Cosmos inicializado");

                var dbBuildings = await _cosmosHelper.GetItemsAsync(b => gescales.Contains(b.id));
                log.Info($"{functionName} En cosmos hay {dbBuildings.Count()}");

                foreach (var building in coverageBuildingList)
                {
                    var dbBuilding = dbBuildings.FirstOrDefault(dbb => dbb.Gescal.Equals(building.Gescal));
                    if (dbBuilding != null)
                    {
                        building.Verticals = dbBuilding.Verticals;
                    }

                    await _cosmosHelper.UpsertDocumentAsync(building, functionName, log).ConfigureAwait(false);
                }

                log.Info($"{functionName} All items inserted");
            }
        }
    }
}
