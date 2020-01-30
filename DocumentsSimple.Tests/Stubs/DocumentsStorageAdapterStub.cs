using DocumentsSimple.Entities;
using DocumentsSimple.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsSimple.Tests.Stubs
{
    internal class DocumentsStorageAdapterStub : IDocumentsStorage
    {
        public DocumentsStorageAdapterStub()
        {
            _documents = new Dictionary<string, Document>();
        }
        private Dictionary<string, Document> _documents;

        public Task Add(Document item)
        {
            _documents[item.Id] = item;
            return Task.FromResult(item);
        }

        public Task Delete(string id)
        {
            return Task.FromResult(_documents.Remove(id));
        }

        public Task<Document> Get(string id)
        {
            return Task.FromResult(_documents[id]);
        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await Task.FromResult(_documents.Values.AsEnumerable());
        }

        public Task Update(string id, Document item)
        {
            return Task.FromResult(_documents[id] = item);
        }
    }
}
