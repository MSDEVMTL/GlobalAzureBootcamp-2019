using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;

namespace GABDemo.Controllers
{
    public class AnalyzerController : Controller
    {
        private readonly IBlobStorageManager _blobStorageManager;
        private readonly IImageAnalyzer _imageAnalyzer;

        public AnalyzerController(IBlobStorageManager blobStorageManager, IImageAnalyzer imageAnalyzer)
        {
            _blobStorageManager = blobStorageManager ?? throw new ArgumentNullException(nameof(blobStorageManager));
            _imageAnalyzer = imageAnalyzer ?? throw new ArgumentNullException(nameof(imageAnalyzer));
        }

        public IActionResult Index()
        {
            var files = _blobStorageManager.GetFiles("images").Select(item => item.Uri).ToList();
            ViewBag.Files = files;
            return View();
        }

        public async Task<IActionResult> Analyze(string imageUrl)
        {
            var results = await _imageAnalyzer.AnalyzeAsync(imageUrl);
            ViewData["Title"] = "Image analysis results";
            ViewData["ImageUrl"] = imageUrl;
            return View("Results", results);
        }
    }
}
