using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using NetConference.Utils.Helpers;

namespace NetConference2018.Functions
{
    public static class UpdateSearchService
    {
        [FunctionName("UpdateSearchService")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                //log.Info("C# HTTP trigger function processed a request.");

                //// parse query parameter
                //string name = req.GetQueryNameValuePairs()
                //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                //    .Value;

                //if (name == null)
                //{
                //    // Get request body
                //    dynamic data = await req.Content.ReadAsAsync<object>();
                //    name = data?.name;
                //}

                await new SearchHelper().UpdateIndexerAsync(log);
                return req.CreateResponse(HttpStatusCode.OK, "Indexr update");
            }
            catch (System.Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }            
        }
    }
}
