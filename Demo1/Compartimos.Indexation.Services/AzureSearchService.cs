using Compartimoss.Indexation.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
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



    }
}
