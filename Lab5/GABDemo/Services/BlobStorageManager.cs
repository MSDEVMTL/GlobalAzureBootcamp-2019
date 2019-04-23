using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GABDemo.Services
{
    public class BlobStorageManager : IBlobStorageManager
    {
        private readonly CloudStorageAccount _storageAccount;

        public BlobStorageManager(IOptions<StorageAccountOptions> options)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            _storageAccount = CreateCloudStorageAccount(options.Value);
        }

        private CloudStorageAccount CreateCloudStorageAccount(StorageAccountOptions options)
        {
            if (!CloudStorageAccount.TryParse(options.ConnectionString, out CloudStorageAccount storageAccount))
            {
                throw new Exception("Invalid storage account connecting string. Please verify the connection string and try again");
            }
            return storageAccount;
        }

        public IEnumerable<IListBlobItem> GetFiles(string containerName)
        {
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);
            var blobs = container.ListBlobs();
            return blobs;
        }
    }
}