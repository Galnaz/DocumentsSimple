using DocumentsSimple.Entities;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Infrastructure
{
    public class DocumentsStorage : IDocumentsStorage
    {
        private Container _container;

        public DocumentsStorage(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task Add(Document item)
        {
            await this._container.CreateItemAsync<Document>(item, new PartitionKey(item.Id));
        }

        public async Task<Document> Get(string id)
        {
            try
            {
                ItemResponse<Document> response = await _container.ReadItemAsync<Document>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            string queryString = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<Document>(new QueryDefinition(queryString));
            List<Document> results = new List<Document>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public Task Update(string id, Document item)
        {
            return _container.UpsertItemAsync<Document>(item, new PartitionKey(id));
        }

        public Task Delete(string id)
        {
            return _container.DeleteItemAsync<Document>(id, new PartitionKey(id));
        }
    }
}
