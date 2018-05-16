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

        #region Groups
        public Task<GroupData[]> GetAllCategoriesAsync()
        {
            return GetGroupsAsync("SELECT cat.name"
                + " FROM c JOIN cat in c.categories"
                + " WHERE c.objectType = 'image'");
        }

        public Task<GroupData[]> GetAllTagsAsync()
        {
            return GetGroupsAsync("SELECT tag.name"
                + " FROM c"
                + " JOIN tag in c.tags"
                + " WHERE c.objectType = 'image'");
        }

        public Task<GroupData[]> GetAllCaptionsAsync()
        {
            return GetGroupsAsync("SELECT caption.text AS name"
                + " FROM c"
                + " JOIN caption in c.captions"
                + " WHERE c.objectType = 'image'");
        }

        private async Task<GroupData[]> GetGroupsAsync(string query)
        {
            var response = await _client.ExecuteStoredProcedureAsync<IDictionary<string, int>>(
                UriFactory.CreateStoredProcedureUri(DB, COLLECTION, "getGroups"),
                _defaultRequestOptions,
                query);
            var result = from p in response.Response
                         select new GroupData
                         {
                             Text = p.Key,
                             Count = p.Value
                         };

            return result.ToArray();
        }
        #endregion

        #region Search
        public async Task<SearchResultData> Search(int maxImageCount, string[] tags)
        {
            var tagFilter = string.Join(
                " OR ",
                from i in Enumerable.Range(1, tags.Length)
                select "tag.name=@tag" + i);
            var tagParams = from i in Enumerable.Range(1, tags.Length)
                            select new SqlParameter("@tag" + i, tags[i - 1]);
            var parameters = CreateParams(
                tagParams.Append(new SqlParameter("@imageCount", maxImageCount)));
            var query = _client.CreateDocumentQuery<SearchImageData>(
                _collectionUri,
                new SqlQuerySpec(
                    "SELECT TOP @imageCount c.id, c.thumbnailUrl, c.captions, c.categories, c.tags"
                    + " FROM c"
                    + (tags.Any() ? " JOIN tag IN c.tags" : string.Empty)
                    + " WHERE c.objectType='image'"
                    + (tags.Any() ? " AND (" + tagFilter + ")" : string.Empty)
                    + " ORDER BY c.captions[0].confidence DESC",
                    parameters),
                _defaultFeedOptions);
            var images = await GetAllResultsAsync(query.AsDocumentQuery());
            var result = new SearchResultData
            {
                Images = images,
                TotalAvailable = 42
            };

            return result;
        }
        #endregion

        private SqlParameterCollection CreateParams(params SqlParameter[] parameters)
        {
            return CreateParams((IEnumerable<SqlParameter>)parameters);
        }

        private SqlParameterCollection CreateParams(IEnumerable<SqlParameter> parameters)
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