using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;
using Microsoft.Extensions.Options;

namespace GABDemo.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly ApiKeysOptions _apiKeys;

        public AnalyzerController(IOptions<ApiKeysOptions> apiKeysOptions)
        {
            _apiKeys = apiKeysOptions.Value;
        }

        public IActionResult Index()
        {
            var manager = new BlobStorageManager(_apiKeys.Storage.ConnectionString);
            var files = manager.GetFiles("images").Select(_ => _.Uri).ToList();

            ViewBag.Files = files;

            return View();
        }

        public async Task<IActionResult> Analyze(string imageUrl)
        {
            var imageAnalyzer = new ImageAnalyzer(_apiKeys.ComputerVision.ApiKey, _apiKeys.ComputerVision.ApiEndPoint);
            var results = await imageAnalyzer.AnalyzeAsync(imageUrl);

            return View("Results",results);
        }
    }
}
