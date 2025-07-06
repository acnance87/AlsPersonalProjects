using BetterBacon8r.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace BetterBacon8r.Controllers {

    public partial class Bacon8rController : Controller {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Bacon8rController> _logger;
        private readonly HttpClient _wikiClient;

        [RegularExpression("<a[^>]*href=[\"']/wiki/[^\"']*[\"'][^>]*>(.*?)<\\/a>")]
        private const string pattern = @"<a[^>]*href=([""'])/wiki/[^""']*[""'][^>]*>(.*?)<\/a>";


        public Bacon8rController(
                IWebHostEnvironment env,
                ILogger<Bacon8rController> logger,
                IHttpClientFactory factory) {
            _env = env;
            _logger = logger;
            _wikiClient = factory.CreateClient("WikiClient");
        }

        public async Task<IActionResult> Index() {
            var seedWords = await GetNewWords();

            return View(seedWords);
        }

        public async Task<IEnumerable<string>> GetNewWords() {
            string filePath = Path.Combine(_env.WebRootPath, "seedWords.json");
            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
            List<string> seedWords = JsonSerializer.Deserialize<List<string>>(jsonContent)!;
            seedWords.ForEach(seedWords => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(seedWords.Trim()));

            return GetTruncatedAndRandomizedWords(seedWords);
        }

        private static List<string> GetTruncatedAndRandomizedWords(List<string> seedWords) {
            var toReturn = new List<string>();
            var kb = seedWords.FirstOrDefault(e => e.Equals("Kevin Bacon", StringComparison.OrdinalIgnoreCase));

            toReturn = [.. seedWords
                .Distinct()
                .OrderBy(x => Guid.NewGuid())
                .Take(40)
                .OrderBy(x => x)];

            if (string.IsNullOrEmpty(kb) is false) {
                toReturn.Add(kb);
                toReturn = toReturn
                    .Distinct()
                    .ToList();
            }

            return toReturn;
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = [nameof(word)])]
        public async Task<IEnumerable<string>> GetNextWords([FromQuery] string word) {
            try {
                var result = await (await _wikiClient.GetAsync(FormatWord(word))).Content.ReadAsStringAsync();
                MatchCollection matches = MyRegex().Matches(result);

                var toReturn = new List<string>();
                toReturn = FilterLinkText(matches.Select(e => e.Groups[2].Value).ToList());

                List<string> FilterLinkText(List<string> wordsToFilter) {
                    return wordsToFilter
                        .Where(e =>
                            e.Contains('<', StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains("span", StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains("CS1", StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains("wikipedia", StringComparison.OrdinalIgnoreCase) is false &&
                            e.Equals(word, StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains("&#", StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains(')', StringComparison.OrdinalIgnoreCase) is false &&
                            e.Contains('(', StringComparison.OrdinalIgnoreCase) is false)
                        .ToList();
                }

                string FormatWord(string word) {
                    return word
                        .Replace(" ", "_")
                        .Replace("/", HttpUtility.UrlEncode("/"))
                        .Replace(",", HttpUtility.UrlEncode(","));
                }

                return GetTruncatedAndRandomizedWords(toReturn);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error fetching next words for: {Word}", word);

                return [];
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [GeneratedRegex(pattern)]
        private static partial Regex MyRegex();
    }
}
