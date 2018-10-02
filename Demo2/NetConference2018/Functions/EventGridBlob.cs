// This is the default URL for triggering event grid function in the local environment.
// http://localhost:7071/admin/extensions/EventGridExtensionConfig?functionName={functionname} 

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json.Linq;
using NetConference.Utils.Helpers;
using System.Threading.Tasks;
using NetConference2018.Shared;

namespace NetConference2018.Functions
{
    public static class EventGridBlob
    {
        private const string functionName = "EventGridBlob";

        [FunctionName(functionName)]
        public static async Task Run([EventGridTrigger]JObject eventGridEvent, TraceWriter log)
        {
            try
            {
                var (action, fileName, isBuilding) = EventGridHelper.GetEventInfo(eventGridEvent);
                
                log.Info($"Detectado el blob {fileName}, al tajo...");

                if (action.Equals("Microsoft.Storage.BlobCreated"))
                {
                    log.Info($"Evento {action}, llega la magia");
                    if (isBuilding)
                    { 
                        log.Info($"{functionName} Es una carga de edificios");
                        await new ProcessBuildings().ProccessByFileAsync(fileName, functionName, log);
                    }
                    else
                    {
                        log.Info($"{functionName} Es una carga de domicilio");
                        await new ProcessHomes().ProccessByFileAsync(fileName, functionName, log);
                    }
                }
                else
                {
                    log.Info($"Evento {action}, no hacemos nada");
                }
            }
            catch (System.Exception ex)
            {
                log.Info($"ERROR {ex.Message}");
            }
        }
    }
}
