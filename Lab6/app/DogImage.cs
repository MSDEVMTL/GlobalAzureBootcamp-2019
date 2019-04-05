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
        private static readonly List<VisualFeatureTypes> Features = new List<VisualFeatureTypes>
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        [FunctionName("DogImage")]
        public static async Task Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob, System.Uri uri, string name, ILogger log)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var visionAPI =  new ComputerVisionClient(new ApiKeyServiceClientCredentials(config["ComputerVision:ApiKey"])) { Endpoint = config["ComputerVision:Endpoint"] };
            var results = await visionAPI.AnalyzeImageAsync(myBlob.Uri.ToString(), Features);
            if(IsDog(results))
            {
                return;
            }
            
            await myBlob.DeleteIfExistsAsync();
        }

        private static bool IsDog(ImageAnalysis image)
        {
            return image.Categories.Any(x => x.Name == "animal_dog") || image.Tags.Any(x => x.Name == "dog");
        }
    }
}
