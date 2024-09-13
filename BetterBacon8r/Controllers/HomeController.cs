using BetterBacon8r.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace BetterBacon8r.Controllers {

    public class HomeController : Controller {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _wikiClient;

        [RegularExpression("<a[^>]*href=[\"']/wiki/[^\"']*[\"'][^>]*>(.*?)<\\/a>")]
        string pattern = @"<a[^>]*href=[""']/wiki/[^""']*[""'][^>]*>(.*?)<\/a>";


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
            var seedWords = await GetNewWords();
                
            return View(seedWords);
        }

        public async Task<IEnumerable<string>> GetNewWords() {
            string webRootPath = _env.WebRootPath;
            string filePath = Path.Combine(webRootPath, "seedWords.json");
            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
            List<string> seedWords = JsonSerializer.Deserialize<List<string>>(jsonContent)!;

            return GetTruncatedAndRandomizedWords(seedWords);
        }

        private static IEnumerable<string> GetTruncatedAndRandomizedWords(List<string> seedWords) {
            return seedWords
                .Distinct()
                .OrderBy(x => Guid.NewGuid())
                .Take(250)
                .OrderBy(x => x)
                .ToList();
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetNextWords([FromQuery]string word) {
            var result = await (await _wikiClient.GetAsync(FormatWord(word))).Content.ReadAsStringAsync();

            MatchCollection matches = Regex.Matches(result, pattern);
            var toReturn = new List<string>();

            foreach (Match match in matches) {
                toReturn.Add(match.Groups[1].Value);
            }

            List<string> FilterLinkText(List<string> wordsToFilter) {
                return wordsToFilter
                    .Where(e => 
                        e.Contains("<", StringComparison.OrdinalIgnoreCase) is false && 
                        e.Contains("span", StringComparison.OrdinalIgnoreCase) is false &&
                        e.Contains("CS1", StringComparison.OrdinalIgnoreCase) is false &&
                        e.Contains("wikipedia", StringComparison.OrdinalIgnoreCase) is false &&
                        e.Equals(word, StringComparison.OrdinalIgnoreCase) is false &&
                        e.Contains("&#", StringComparison.OrdinalIgnoreCase) is false)
                    .ToList();
            }

            string FormatWord(string word) {
                return word.Replace(" ", "_");
            }

            return GetTruncatedAndRandomizedWords(FilterLinkText(toReturn));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
