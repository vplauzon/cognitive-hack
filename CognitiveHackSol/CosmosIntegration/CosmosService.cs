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
        public async Task<SearchResultData> SearchAsync(
            int maxImageCount,
            string[] tags,
            string[] categories,
            string[] captions)
        {
            var tagFilter = CreateSearchFilter("tags", "name", tags);
            var categoryFilter = CreateSearchFilter("categories", "name", categories);
            var captionFilter = CreateSearchFilter("captions", "text", captions);
            var activeFilters = from s in
                                  new[] { tagFilter.Filter, categoryFilter.Filter, captionFilter.Filter }
                                where !string.IsNullOrWhiteSpace(s)
                                select s;
            var activeFilterFormat = string.Join(" OR ", activeFilters);
            var parameters = CreateParams(tagFilter.Parameters
                .Concat(categoryFilter.Parameters)
                .Concat(captionFilter.Parameters)
                .Append(new SqlParameter("@imageCount", maxImageCount)));
            var queryFromText = " FROM c"
                + " WHERE c.objectType='image'"
                + (activeFilters.Any() ? " AND (" + activeFilterFormat + ")" : string.Empty)
                + " ORDER BY c.captions[0].confidence DESC";
            var queryTopText =
                "SELECT TOP @imageCount c.id, c.thumbnailUrl, c.captions, c.categories, c.tags"
                + queryFromText;
            var queryTop = _client.CreateDocumentQuery<SearchImageData>(
                _collectionUri,
                new SqlQuerySpec(queryTopText, parameters),
                _defaultFeedOptions);
            var imagesTask = GetAllResultsAsync(queryTop.AsDocumentQuery());
            var queryCountText = "SELECT VALUE COUNT(c.id)" + queryFromText;
            var queryCount = _client.CreateDocumentQuery<int>(
                _collectionUri,
                new SqlQuerySpec(queryCountText, parameters),
                _defaultFeedOptions);
            var countTask = GetAllResultsAsync(queryCount.AsDocumentQuery());

            await Task.WhenAll(imagesTask, countTask);

            var images = imagesTask.Result;
            var count = countTask.Result;
            var result = new SearchResultData
            {
                Images = images,
                TotalAvailable = count[0]
            };

            return result;
        }

        private (string Filter, IEnumerable<SqlParameter> Parameters) CreateSearchFilter(
            string docField,
            string fieldTextName,
            string[] fieldValueList)
        {
            if (fieldValueList == null || fieldValueList.Length == 0)
            {
                return (string.Empty, new SqlParameter[0]);
            }
            else
            {
                var paramNameList = from i in Enumerable.Range(1, fieldValueList.Length)
                                    select "@" + docField + i;
                var paramFormattedList = string.Join(", ", paramNameList);
                var filter = $"EXISTS(SELECT VALUE t FROM t IN c.{docField} WHERE"
                    + $" t.{fieldTextName} IN ({paramFormattedList}))";
                var paramList = from i in Enumerable.Range(1, fieldValueList.Length)
                                select new SqlParameter("@" + docField + i, fieldValueList[i - 1]);

                return (filter, paramList);
            }
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