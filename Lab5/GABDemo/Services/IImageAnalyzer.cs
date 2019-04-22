using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace GABDemo.Services
{
    public interface IImageAnalyzer
    {
        Task<ImageAnalysis> AnalyzeAsync(string imageUrl);
    }
}