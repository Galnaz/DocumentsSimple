using DocumentsSimple.Config;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace DocumentsSimple.Infrastructure
{
    public class StorageFactory
    {
        private CloudBlobClient _cloudBlobClient;

        public StorageFactory(IOptions<StorageConfig> options)
        {
            CloudStorageAccount storageAccount =
               CreateStorageAccountFromConnectionString(options.Value.StorageConnection);
            var storageClient = storageAccount.CreateCloudBlobClient();
            _cloudBlobClient = storageClient;
        }

        public async Task<CloudBlobContainer> GetCloudBlobContainer(string blobContainerName)
        {
            var container = _cloudBlobClient.GetContainerReference(blobContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                //log instead
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}