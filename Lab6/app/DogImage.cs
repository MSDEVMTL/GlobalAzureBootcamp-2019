using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        public static void Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, System.Uri uri, string name, ILogger log)
        {
            // var config = new ConfigurationBuilder()
            //     .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //     .AddEnvironmentVariables()
            //     .Build();
            // var visionAPI =  new ComputerVisionClient(new ApiKeyServiceClientCredentials(config[""])) { Endpoint = config[] };


            log.LogInformation($"C# Blob trigger function Processed blob\n URI:{uri} \n Name:{name} \n Size: {myBlob.Length} Bytes");
            // call visionAPI here
            
            // delete if not dog
        }
    }
}
