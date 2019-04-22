using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GABDemo.Services
{
    public interface IBlogStorageManager
    {
        IEnumerable<IListBlobItem> GetFiles(string containerName);
    }
}