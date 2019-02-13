using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;
using Microsoft.Extensions.Options;

namespace GABDemo.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly KeysOptions _keys;

        public AnalyzerController(IOptions<KeysOptions> apiKeysOptions)
        {
            _keys = apiKeysOptions.Value;
        }

        public IActionResult Index()
        {
            var manager = new BlobStorageManager(_keys.Storage.ConnectionString);
            var files = manager.GetFiles("images").Select(_ => _.Uri).ToList();

            ViewBag.Files = files;

            return View();
        }

        public async Task<IActionResult> Analyze(string imageUrl)
        {
            var imageAnalyzer = new ImageAnalyzer(_keys.ComputerVision.ApiKey, _keys.ComputerVision.ApiEndPoint);
            var results = await imageAnalyzer.AnalyzeAsync(imageUrl);
            ViewData["Title"] = "Image analysis results";

            return View("Results",results);
        }
    }
}
