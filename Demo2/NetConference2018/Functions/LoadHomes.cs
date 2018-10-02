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
    public static class LoadHomes
    {
        private const string functionName = "LoadHomes";

        [Disable]
        [FunctionName(functionName)]
        public static async Task Run([BlobTrigger("domicilios/{name}.csv", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            try
            {
                var homesList = NetConference.Utils.Helpers.CsvHelper.ReadCsvFileWithMapperAndHeader<VerticalsCsv, VerticalsCsvMap>(myBlob);
                await new ProcessHomes().ProccessByItemListAsync(homesList, functionName, log);
                //await new ProcessHomes().ProccessByFileAsync($"{name}.csv", functionName, log);
            }
            catch (Exception ex)
            {
                log.Error($"{functionName} {ex.Message}");
            }
        }
    }
}
