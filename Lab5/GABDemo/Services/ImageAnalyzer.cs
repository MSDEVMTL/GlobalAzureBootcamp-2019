using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace GABDemo.Services
{
    public class ImageAnalyzer : IImageAnalyzer
    {
        private readonly ComputerVisionClient _computerVision;

        private static readonly List<VisualFeatureTypes> Features =
            new List<VisualFeatureTypes>
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags
            };

        public ImageAnalyzer(ComputerVisionClient computerVision)
        {
            _computerVision = computerVision ?? throw new ArgumentNullException(nameof(computerVision));
        }

        public Task<ImageAnalysis> AnalyzeAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new Exception($"Invalid remoteImageUrl: {imageUrl}");
            }
            return _computerVision.AnalyzeImageAsync(imageUrl, Features);
        }
    }
}
