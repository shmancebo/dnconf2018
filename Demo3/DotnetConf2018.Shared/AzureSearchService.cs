using DotnetConf2018.Services.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compartimos.Indexation.Services
{

    public class AzureSearchService
    {
        private readonly SearchServiceClient searchServiceClient;

        public AzureSearchService(string searchServiceName,string adminApiKey)
        {
            searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }


        public async Task<Index> CreateIndexAsync<T>(string indexName, bool overwriteIfExists, List<Suggester> suggesters) where T : class
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<T>(),
                Suggesters = suggesters
            };

            var existIndex = await searchServiceClient.Indexes.ExistsAsync(indexName).ConfigureAwait(false);

            if (!existIndex)
            {
                return await searchServiceClient.Indexes.CreateAsync(definition).ConfigureAwait(false);
            }
            if (existIndex && overwriteIfExists)
            {
                await searchServiceClient.Indexes.DeleteAsync(indexName).ConfigureAwait(false);

                return await searchServiceClient.Indexes.CreateAsync(definition).ConfigureAwait(false);
            }

            return await searchServiceClient.Indexes.GetAsync(indexName).ConfigureAwait(false);
        }

        public async Task<KeyValuePair<bool, IndexBatch<T>>> UploadDocuments<T>(string indexName, T[] array) where T : AzureSearchModel
        {
            var searchIndexClient = searchServiceClient.Indexes.GetClient(indexName);
            var batch = IndexBatch.MergeOrUpload(array);
            var retry = true;
            var count = 0;
            try
            {
                await searchIndexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
            }
            catch (IndexBatchException e)
            {

                var retryBatch = e.FindFailedActionsToRetry(batch, arg => arg.Id);

                while (retry)
                {
                    try
                    {
                        await searchIndexClient.Documents.IndexAsync(retryBatch).ConfigureAwait(false);
                        retry = false;
                    }
                    catch (IndexBatchException exception)
                    {

                        count++;
                        retryBatch = exception.FindFailedActionsToRetry(retryBatch, arg => arg.Id);
                        if (count == 5)
                        {
                            return new KeyValuePair<bool, IndexBatch<T>>(false, retryBatch);
                        }

                    }
                }
            }
            return new KeyValuePair<bool, IndexBatch<T>>(true, batch);
        }

        public async Task<DotnetConf2018.Services.Models.SearchResult<T>> RunQuery<T>(string indexName,
                                             SearchParameters searchParameters,
                                             string key,
                                             SearchContinuationToken continuationToken,
                                             bool highScoring = false) where T : class
        {
            var results = new List<T>();
            var queryResult = new DotnetConf2018.Services.Models.SearchResult<T>();

            try
            {
                var searchIndexClient = searchServiceClient.Indexes.GetClient(indexName);
                var headers = new Dictionary<string, List<string>> { { "x-ms-azs-return-searchid", new List<string> { "true" } } };

                AzureOperationResponse<DocumentSearchResult<T>> azureOperationsResponse;

                if (continuationToken != null)
                {
                    azureOperationsResponse = await searchIndexClient.Documents.ContinueSearchWithHttpMessagesAsync<T>(continuationToken).ConfigureAwait(false);
                }
                else
                {
                    azureOperationsResponse = await searchIndexClient.Documents.SearchWithHttpMessagesAsync<T>(key, searchParameters, customHeaders: headers).ConfigureAwait(false);
                }

                if (azureOperationsResponse.Response.IsSuccessStatusCode)
                {
                    var searchResults = azureOperationsResponse.Body.Results;
                    queryResult.Facet = azureOperationsResponse.Body.Facets;
                    if (highScoring && searchResults.Any(i => i.Score > 2))
                    {
                        results.AddRange(searchResults.Where(i => i.Score > 2).Select(i => i.Document));
                    }
                    else
                    {
                        results.AddRange(searchResults.Select(i => i.Document));
                    }

                    queryResult.ContinuationToken = azureOperationsResponse.Body.ContinuationToken;
                    queryResult.Collection = results;

                }
            }
            catch (Exception ex)
            {

            }

            return queryResult;
        }
    }
}
