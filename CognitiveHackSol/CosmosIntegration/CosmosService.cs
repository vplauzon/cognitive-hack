using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace CosmosIntegration
{
    public class CosmosService
    {
        private readonly DocumentClient _client;
        private readonly Uri _collectionUri;
        private readonly SqlParameter _sessionId;

        public CosmosService(Uri endpoint, string key, string sessionId)
        {
            _client = new DocumentClient(endpoint, key);
            _collectionUri = UriFactory.CreateDocumentCollectionUri("mydb", "images");
            _sessionId = new SqlParameter("@sessionId", sessionId);
        }

        public async Task<string> GetStatusAsync()
        {
            var query = _client.CreateDocumentQuery(
                _collectionUri,
                new SqlQuerySpec(
                    "SELECT c.status FROM c WHERE c.objectType='session' AND c.session=@sessionId",
                    CreateParams(_sessionId))
                );
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

        public async Task<int> GetPhotoCountAsync()
        {
            var query = _client.CreateDocumentQuery<int>(
                _collectionUri,
                new SqlQuerySpec(
                    "SELECT VALUE COUNT(c) FROM c WHERE c.objectType='image' AND c.session=@sessionId",
                    CreateParams(_sessionId))
                );
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