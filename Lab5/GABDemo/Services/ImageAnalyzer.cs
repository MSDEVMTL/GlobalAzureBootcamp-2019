using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace GABDemo.Services
{
    public class ImageAnalyzer
    {
        private readonly ComputerVisionClient _computerVision;

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> Features =
            new List<VisualFeatureTypes>
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags
            };

        public ImageAnalyzer(string apiKey, string apiEndpoint)
        {
            _computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey));
            _computerVision.Endpoint = apiEndpoint;
        }

        public async Task<ImageAnalysis> AnalyzeAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new Exception("Invalid remoteImageUrl: {imageUrl}");
            }

            ImageAnalysis analysis = await _computerVision.AnalyzeImageAsync(imageUrl, Features);
            return analysis;
        }
    }
}
