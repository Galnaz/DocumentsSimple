using DocumentsSimple.Entities;
using DocumentsSimple.Exceptions;
using DocumentsSimple.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Application
{
    public class DocumentsService
    {
        private IBlobStorageAdapter _blob;
        private readonly IDocumentsStorage _documentsStorage;
        public DocumentsService(IBlobStorageAdapter blobStorage, IDocumentsStorage cosmosDbService)
        {
            _blob = blobStorage;
            _documentsStorage = cosmosDbService;
        }
        public Task<IEnumerable<Document>> GetAll()
        {
            return _documentsStorage.GetAll();
        }
        public Task<Document> Get(string id)
        {
            return _documentsStorage.Get(id);
        }

        public async Task<Document> Upload(IFormFile file, string downloadLocation)
        {
            // This logic would much better fit inot domain layer. Since time is fixed, let's put it here and move to proper place later
            if (file.ContentType != "application/pdf")
            {
                throw new HttpStatusCodeException(StatusCodes.Status400BadRequest, "file should be in pdf format");
            }

            if (file.Length > 5000000)
            {
                throw new HttpStatusCodeException(StatusCodes.Status400BadRequest, "file size is too large");
            }

            var name = Guid.NewGuid().ToString();

            var res = await _blob.Upload(file, name);
            var document = new Document
            {
                Id = name,
                Name = file.FileName,
                Size = file.Length,
                Location = $"{downloadLocation}/{name}"
            };
            await _documentsStorage.Add(document);
            return document;
        }

        public Task Delete(string id)
        {
            return _documentsStorage.Delete(id);
        }
    }
}
