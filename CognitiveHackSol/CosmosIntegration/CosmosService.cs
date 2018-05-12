using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace CosmosIntegration
{
    public class CosmosService
    {
        private const string DB = "mydb";
        private const string COLLECTION = "images";

        private readonly DocumentClient _client;
        private readonly Uri _collectionUri;
        private readonly FeedOptions _defaultFeedOptions;
        private readonly RequestOptions _defaultRequestOptions;

        public CosmosService(Uri endpoint, string key, string sessionId)
        {
            _client = new DocumentClient(endpoint, key);
            _collectionUri = UriFactory.CreateDocumentCollectionUri(DB, COLLECTION);
            _defaultFeedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(sessionId)
            };
            _defaultRequestOptions = new RequestOptions
            {
                PartitionKey = _defaultFeedOptions.PartitionKey
            };
        }

        public async Task<string> GetStatusAsync()
        {
            var query = _client.CreateDocumentQuery(
                _collectionUri,
                "SELECT c.status FROM c"
                + " WHERE c.objectType='session'",
               _defaultFeedOptions);
            var result = await GetAllResultsAsync(query.AsDocumentQuery());

            if (result.Length == 1)
            {
                return result[0].status as string;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<int> GetImageCountAsync()
        {
            var query = _client.CreateDocumentQuery<int>(
                _collectionUri,
                "SELECT VALUE COUNT(c) FROM c"
                + " WHERE c.objectType='image'",
                _defaultFeedOptions);
            var result = await GetAllResultsAsync<int>(query.AsDocumentQuery());

            if (result.Length == 1)
            {
                return result[0];
            }
            else
            {
                return 0;
            }
        }

        public async Task<GroupData[]> GetAllCaptionsAsync()
        {
            var response = await _client.ExecuteStoredProcedureAsync<IDictionary<string, int>>(
                UriFactory.CreateStoredProcedureUri(DB, COLLECTION, "getAllCaptions"),
                _defaultRequestOptions);
            var result = from p in response.Response
                         select new GroupData
                         {
                             Name = p.Key,
                             Count = p.Value
                         };

            return result.ToArray();
        }

        public async Task<SearchImageData[]> SearchNoCriteriaAsync(int imageCount)
        {
            var query = _client.CreateDocumentQuery<SearchImageData>(
                _collectionUri,
                new SqlQuerySpec(
                    "SELECT TOP @imageCount c.thumbnailUrl, c.captions, c.categories, c.tags"
                    + " FROM c"
                    + " WHERE c.objectType='image'"
                    + " ORDER BY c.captions[0].confidence DESC",
                    CreateParams(new SqlParameter("@imageCount", imageCount))),
                _defaultFeedOptions);
            var result = await GetAllResultsAsync(query.AsDocumentQuery());

            return result;
        }

        private SqlParameterCollection CreateParams(params SqlParameter[] parameters)
        {
            return new SqlParameterCollection(parameters);
        }

        private async Task<T[]> GetAllResultsAsync<T>(IDocumentQuery<T> query)
        {
            var list = new List<T>();

            while (query.HasMoreResults)
            {
                var docs = await query.ExecuteNextAsync<T>();

                list.AddRange(docs);
            }

            return list.ToArray();
        }
    }
}