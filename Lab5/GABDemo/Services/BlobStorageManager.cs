using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GABDemo.Services
{
    public class BlobStorageManager
    {
        private readonly CloudStorageAccount _storageAccount;

        public BlobStorageManager(string connectionString)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out _storageAccount))
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