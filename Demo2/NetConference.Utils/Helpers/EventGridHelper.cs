using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetConference.Utils.Helpers
{
    public static class EventGridHelper
    {
        public static (string action, string fileName, bool isStreet) GetEventInfo(JObject eventGridEvent)
        {
            return (action: (string)eventGridEvent["eventType"], 
                fileName: System.IO.Path.GetFileName((string)eventGridEvent["data"]["url"]),
                isStreet: ((string)eventGridEvent["data"]["url"]).Contains("edificios"));
        }
    }
}
