using DocumentsSimple.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Infrastructure
{
    public class BlobStorageAdapter : IBlobStorageAdapter
    {
        private StorageFactory _storageFactory;
        private readonly string _containerName;

        public BlobStorageAdapter(StorageFactory factory, IOptions<StorageConfig> options)
        {
            _containerName = options.Value.BlobContainer;
            _storageFactory = factory;
        }

        public async Task<string> Upload(IFormFile file, string name)
        {
            var _cloudBlobContainer = await _storageFactory.GetCloudBlobContainer(_containerName);
            var blob = _cloudBlobContainer.GetBlockBlobReference(name);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);
            }

            return $"{_cloudBlobContainer.Uri.AbsoluteUri}/{name}";
        }

        public async Task<Stream> DownloadFile(string name)
        {
            var _cloudBlobContainer = await _storageFactory.GetCloudBlobContainer(_containerName);
            var resultStream = new MemoryStream();
            await _cloudBlobContainer.CreateIfNotExistsAsync();
            var blobFile = _cloudBlobContainer.GetBlobReference(name);
            var fileExists = await blobFile.ExistsAsync();
            if (!fileExists)
                return null;
            await blobFile.DownloadToStreamAsync(resultStream);
            return await blobFile.OpenReadAsync();
        }
    }
}
