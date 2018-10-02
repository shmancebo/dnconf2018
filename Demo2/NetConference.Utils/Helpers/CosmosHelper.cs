using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs.Host;
using NetConference.Utils.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NetConference.Utils.Helpers
{
    public class CosmosHelper<T> where T : CoverageBuilding
    {
        private readonly string DatabaseId = ConfigurationManager.AppSettings["CosmosDBName"];
        private readonly string CollectionId = ConfigurationManager.AppSettings["CollectionName"];
        private readonly string CosmosDBEndpointUrl = ConfigurationManager.AppSettings["CosmosDBEndpointUrl"];
        private readonly string CosmosDBPrimaryKey = ConfigurationManager.AppSettings["CosmosDBPrimaryKey"];
        private DocumentClient client;

        public async Task InitializeAsync(TraceWriter log)
        {
            log.Info($"{CosmosDBEndpointUrl}");
            log.Info($"{CosmosDBPrimaryKey}");
            client = new DocumentClient(new Uri(CosmosDBEndpointUrl), CosmosDBPrimaryKey);
            await CreateDatabaseIfNotExistsAsync();
            log.Info("Validada la BBDD");
            await CreateCollectionIfNotExistsAsync();
            log.Info("Validada la coleccion");
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                feedOptions: new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task UpsertDocumentAsync(T building, string functionName, TraceWriter log)
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
                var res = await client.UpsertDocumentAsync(uri, building, null, true);

                log.Info($"{functionName}, {building.Gescal} | processed in cosmosDB with statusCode: x{((int)res.StatusCode).ToString()}-{res.StatusCode}");
            }
            catch (DocumentClientException e)
            {
                log.Info($"{functionName}, {building.Gescal} | no processed in cosmosDB with errorCode: x{((int)e.StatusCode).ToString()}-{ e.StatusCode}");
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection
                        {
                            Id = CollectionId
                        },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
