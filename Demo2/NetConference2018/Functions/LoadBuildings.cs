using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NetConference.Utils.CsvMappers;
using NetConference.Utils.Models;
using NetConference2018.Shared;

namespace NetConference2018.Functions
{
    public static class LoadBuildings
    {
        private const string functionName = "LoadBuildings";

        [Disable]
        [FunctionName(functionName)]
        public static async Task Run([BlobTrigger("edificios2222/{name}.csv", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            try
            {
                var coverageBuildingList = NetConference.Utils.Helpers.CsvHelper.ReadCsvFileWithMapperAndHeader<CoverageBuilding, CoverageBuildingCsvMap>(myBlob);
                await new ProcessBuildings().ProccessByItemListAsync(coverageBuildingList, functionName, log);
                //await new ProcessBuildings().ProccessByFileAsync($"{name}.csv", functionName, log);
            }
            catch (Exception ex)
            {
                log.Error($"{functionName} {ex.Message}");
            }
        }
    }  
}
