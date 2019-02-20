using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;
using Microsoft.Extensions.Options;

namespace GABDemo.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly IBlogStorageManager _blobStorageManager;
        private readonly IImageAnalyzer _imageAnalyzer;

        public AnalyzerController(IBlogStorageManager blobStorageManager, IImageAnalyzer imageAnalyzer)
        {
            _blobStorageManager = blobStorageManager ?? throw new ArgumentNullException(nameof(blobStorageManager));
            _imageAnalyzer = imageAnalyzer ?? throw new ArgumentNullException(nameof(imageAnalyzer));
        }

        public IActionResult Index()
        {
            var files = _blobStorageManager.GetFiles("images").Select(_ => _.Uri).ToList();

            ViewBag.Files = files;

            return View();
        }

        public async Task<IActionResult> Analyze(string imageUrl)
        {
            var results = await _imageAnalyzer.AnalyzeAsync(imageUrl);
            ViewData["Title"] = "Image analysis results";

            return View("Results",results);
        }
    }
}
