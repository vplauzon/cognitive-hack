using Microsoft.Azure.Documents.Client;
using System;
using System.Security;

namespace CosmosIntegration
{
    public class CosmosService
    {
        private readonly DocumentClient _client;
        private readonly string _sessionId;

        public CosmosService(Uri endpoint, string key, string sessionId)
        {
            _client = new DocumentClient(endpoint, key);
            _sessionId = sessionId;
        }
    }
}