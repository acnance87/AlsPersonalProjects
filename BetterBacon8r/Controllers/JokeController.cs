using Microsoft.AspNetCore.Mvc;
using AlsProjects.Controllers.API;
using AlsProjects.Model;
using AlsProjects.Service;

namespace AlsProjects.Controllers {
    [Route("home")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class JokeController : Controller {
        private readonly IJokesApiController _jokesApi;
        private readonly IWebHostEnvironment _env;
        private readonly JokesInMemoryDatabaseSaver _jokesInMemoryDatabaseSaver;
        private readonly ILogger<JokeController> _logger;

        public JokeController(
            IJokesApiController testApiController,
            IWebHostEnvironment env,
            ILogger<JokeController> logger) {
            _jokesApi = testApiController;
            _env = env;
            _jokesInMemoryDatabaseSaver = new JokesInMemoryDatabaseSaver(env);
            _logger = logger;
        }

        [HttpGet]
        [Route(nameof(Index))]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index() {
            var topics = await _jokesApi.GetTopics();
            return View(topics);
        }

        [HttpGet]
        [Route(nameof(GetJoke))]
        public async Task<ActionResult<JokesDto>> GetJoke(string? topic = null, bool isHeardPhrase = false) {
            var joke = await _jokesApi.Get(topic, isHeardPhrase);
            return View(joke);
        }

        [HttpPost]
        [Route(nameof(CreateJoke))]
        public async Task<ActionResult> CreateJoke([FromForm] string joke, string? topic = null) {
            if (TryValidateModel(joke) is false) {
                return BadRequest();
            }
            try {
                var newJoke = new JokesDto() { Joke = joke, Topic = topic! };
                var added = await _jokesApi.Create(newJoke);
                await SaveJokesToFile(await _jokesApi.GetAllJokes());

                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error creating joke");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route(nameof(GetAllJokes))]
        public async Task<ActionResult> GetAllJokes() {
            return View(await _jokesApi.GetAllJokes());
        }

        [HttpPost]
        [Route(nameof(DeleteJoke))]
        public async Task DeleteJoke(int id) {
            var isDeleted = await _jokesApi.DeleteJoke(id);
            if (isDeleted is false) {
                throw new InvalidCastException($"There was an unexpected problem deleting the joke with Id = {id}");
            }
        }

        [HttpGet]
        [Route(nameof(Listen))]
        public async Task<IActionResult> Listen() {
            var basePath = _env.ContentRootPath;
            var modelPath = Path.Combine(basePath, nameof(Speech), nameof(Model), "vosk-model-en-us-0.15");
            var heardStatement = await Speech.Utility.Listen(modelPath);

            return RedirectToAction(nameof(GetJoke), new { topic = heardStatement, isHeardPhrase = true });
        }

        [HttpGet]
        [Route(nameof(Speak))]
        public async Task<IActionResult> Speak(string text) {
            Speech.Utility.SpeakText(text);

            return await Task.FromResult(RedirectToAction("Index"));
        }

        private async Task SaveJokesToFile(IEnumerable<Jokes> jokes) {
            try {
                await _jokesInMemoryDatabaseSaver.SaveJokesData(jokes);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error saving jokes to file");
            }
        }
    }
}
