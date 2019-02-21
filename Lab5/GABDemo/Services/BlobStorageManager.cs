using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GABDemo.Services
{
    public class BlobStorageManager : IBlogStorageManager
    {
        private readonly CloudStorageAccount _storageAccount;

        public BlobStorageManager(StorageAccountOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new Exception("Storage connection string is missing");
            }

            if (!CloudStorageAccount.TryParse(options.ConnectionString, out _storageAccount))
            {
                throw new Exception(
                    "Invalid storage account connecting string. Please verify the connection string and try again");
            }
        }

        public IEnumerable<IListBlobItem> GetFiles(string containerName)
        {
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();

            var container = cloudBlobClient.GetContainerReference(containerName);
            foreach (var file in container.ListBlobs())
            {
                yield return file;
            }
        }
    }
}