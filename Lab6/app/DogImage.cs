using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace app
{
    public static class DogImage
    {
        // Feature we want to work with when getting analysis back
        private static readonly List<VisualFeatureTypes> Features = new List<VisualFeatureTypes>
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        // We must provide SAS token in order to have the API read the image located at the provided URL since our container is private
        private static SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
        {
            SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10),
            Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
        };


        [FunctionName("DogImage")]
        public static async Task Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob, string name, ILogger log)
        {
            // FUNCTION CODE HERE
        }

        private static bool IsDog(ImageAnalysis image)
        {
            return image.Categories.Any(x => x.Name == "animal_dog") || image.Tags.Any(x => x.Name == "dog");
        }
    }
}
