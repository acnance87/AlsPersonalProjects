using AlsProjects.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AlsProjects.Controllers {
    public class JavascriptOrNah : Controller {

        private readonly IWebHostEnvironment _env;

        public JavascriptOrNah(IWebHostEnvironment env) {
            _env = env;
        }
        public async Task<IActionResult> Index() {
            var viewModel = new JavascriptOrNahViewModel() {
                RandomWords = await GetSeedData("seedWords.json"),
                JavascriptWords = await GetSeedData("javascriptNames.json")
            };

            return View(viewModel);
        }

        private async Task<IEnumerable<string>> GetSeedData(string fileName) {
            string webRootPath = _env.WebRootPath;
            string filePath = Path.Combine(webRootPath, fileName);
            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);

            return JsonSerializer.Deserialize<List<string>>(jsonContent)!;
        }
    }
}
