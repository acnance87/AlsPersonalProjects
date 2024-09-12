using BetterBacon8r.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace BetterBacon8r.Controllers {

    public class HomeController : Controller {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _wikiClient;

        public HomeController(
            IWebHostEnvironment env,
            ILogger<HomeController> logger,
            IHttpClientFactory factory
            ) {
            _env = env;
            _logger = logger;
            _wikiClient = factory.CreateClient("WikiClient");
        }

        public async Task<IActionResult> Index() {
            string webRootPath = _env.WebRootPath;
            string filePath = Path.Combine(webRootPath, "seedWords.json");
            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
            List<string> seedWords = JsonSerializer.Deserialize<List<string>>(jsonContent)!;

            return View(seedWords);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
