using Microsoft.Azure.WebJobs.Host;
using NetConference.Utils.CsvMappers;
using NetConference.Utils.Helpers;
using NetConference.Utils.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetConference2018.Shared
{
    public class ProcessHomes
    {
        readonly CosmosHelper<CoverageBuilding> _cosmosHelper;
        public ProcessHomes()
        {
            _cosmosHelper = new CosmosHelper<CoverageBuilding>();
        }

        public async Task ProccessByFileAsync(string fileName, string functionName, TraceWriter log)
        {
            log.Info($"{functionName} Recuperando el fichero");
            IEnumerable<VerticalsCsv> verticalsList = null;
            using (var ms = BlobHelper.GetStream(fileName, "domicilios"))
            {
                verticalsList = NetConference.Utils.Helpers.CsvHelper.ReadCsvFileWithMapperAndHeader<VerticalsCsv, VerticalsCsvMap>(ms).ToList();
            }

            await ProccessByItemListAsync(verticalsList, functionName, log);
        }

        public async Task ProccessByItemListAsync(IEnumerable<VerticalsCsv> verticalList, string functionName, TraceWriter log)
        {
            if (verticalList.Any())
            {
                await _cosmosHelper.InitializeAsync(log);
                log.Info($"{functionName} Cosmos inicializado");

                var groupByBuilding = verticalList.GroupBy(x => x.Gescal).ToList();
                var buildingsInList = groupByBuilding.Select(gbd => gbd.Key).ToList();
                var dbBuildings = await _cosmosHelper.GetItemsAsync(b => buildingsInList.Contains(b.id));

                foreach(var buildingGroup in groupByBuilding)
                {
                    var dbBuilding = dbBuildings.FirstOrDefault(dbb => dbb.Gescal.Equals(buildingGroup.Key));
                    if (dbBuilding != null)
                    {
                        var homesSerialized = GetHomesForBuilding(buildingGroup.ToList());
                        dbBuilding.Verticals = homesSerialized;

                        await _cosmosHelper.UpsertDocumentAsync(dbBuilding, functionName, log);
                        log.Info($"{functionName} {dbBuilding.Gescal} homes updated");
                    }
                }

                log.Info($"{functionName} All items inserted");
            }
        }

        string GetHomesForBuilding(List<VerticalsCsv> verticalsCsvList)
        {
            var verticalsList = new List<Verticals>();
            List<VerticalsTechnology> verticalTechnologyList;

            foreach (var verticalItem in verticalsCsvList.GroupBy(x => x.Gescal37))
            {
                var verticalResponse = CastHelper.GetRespuestaVertical(verticalItem.First().Gescal37);

                Verticals verticalMapped = MappingBasicInfoVertical(verticalResponse);

                verticalTechnologyList = verticalItem.ToList().Mapper<VerticalsCsv, VerticalsTechnology>();
                verticalMapped.Technologies = new List<VerticalsTechnology>();
                verticalMapped.Technologies.AddRange(verticalTechnologyList);
                verticalsList.Add(verticalMapped);
            }
            return JsonConvert.SerializeObject(verticalsList);
        }

        Verticals MappingBasicInfoVertical(RespuestaVertical verticalResponse)
        {
            return new Verticals()
            {
                Gescal37 = verticalResponse.gescal37,
                Block = verticalResponse.bloque,
                BlockId = verticalResponse.bloqueId,
                Bisduplicate = verticalResponse.bis,
                BisduplicateId = verticalResponse.bisId,
                Door = verticalResponse.portalPuerta,
                DoorId = verticalResponse.portalPuertaId,
                Floor = verticalResponse.planta,
                FloorId = verticalResponse.plantaId,
                Letter = verticalResponse.letraFinca,
                Stair = verticalResponse.escalera,
                StairId = verticalResponse.escaleraId,
                Hand1 = verticalResponse.mano1,
                Hand2 = verticalResponse.mano2,
            };
        }
    }
}
