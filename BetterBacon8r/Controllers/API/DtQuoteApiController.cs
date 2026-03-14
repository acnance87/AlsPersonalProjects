using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace AlsProjects.Controllers.API {
    [ApiController]
    [Route("api/dtquote")]
    public class DtQuoteApiController : ControllerBase {
        private readonly IWebHostEnvironment _env;

        public DtQuoteApiController(IWebHostEnvironment env) {
            _env = env;
        }

        // GET api/dtquote?k=heresy
        [ResponseCache]
        [HttpGet]
        public ActionResult<string> Get([FromQuery] string? k) {
            // Only return data when the key k equals the expected value
            if (string.IsNullOrWhiteSpace(k) || k != "heresy") {
                return NoContent();
            }

            // Try multiple possible locations for the file
            string? path = null;

            // First try: wwwroot/data/dtquotes.txt
            if (!string.IsNullOrEmpty(_env.WebRootPath)) {
                var webRootPath = Path.Combine(_env.WebRootPath, "data", "dtquotes.txt");
                if (System.IO.File.Exists(webRootPath)) {
                    path = webRootPath;
                }
            }

            // Second try: ContentRootPath/wwwroot/data/dtquotes.txt
            if (path == null) {
                var contentPath = Path.Combine(_env.ContentRootPath, "wwwroot", "data", "dtquotes.txt");
                if (System.IO.File.Exists(contentPath)) {
                    path = contentPath;
                }
            }

            // Third try: ContentRootPath/dtquotes.txt (fallback for local development)
            if (path == null) {
                var rootPath = Path.Combine(_env.ContentRootPath, "dtquotes.txt");
                if (System.IO.File.Exists(rootPath)) {
                    path = rootPath;
                }
            }

            if (path == null || !System.IO.File.Exists(path))
                return NotFound();

            var lines = System.IO.File.ReadAllLines(path)
                .Where(l => string.IsNullOrWhiteSpace(l) is false)
                .Select(l => l.Trim())
                .ToList();

            if (lines.Count == 0)
                return NotFound();

            // Return a random quote
            var random = new Random();
            var randomQuote = lines[random.Next(lines.Count)];

            return Ok(randomQuote);
        }
    }
}
