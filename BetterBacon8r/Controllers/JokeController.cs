using Microsoft.AspNetCore.Mvc;
using AlsProjects.Controllers.API;
using AlsProjects.Model;

namespace AlsProjects.Controllers
{
    [Route("home")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class JokeController : Controller
    {
        private readonly IJokesApiController _jokesApi;
        private readonly IWebHostEnvironment _env;

        public JokeController(IJokesApiController testApiController, IWebHostEnvironment env)
        {
            _jokesApi = testApiController;
            _env = env;
        }

        [HttpGet]
        [Route(nameof(Index))]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            var topics = await _jokesApi.GetTopics();
            return View(topics);
        }

        [HttpGet]
        [Route(nameof(GetJoke))]
        public async Task<ActionResult<JokesDto>> GetJoke(string? topic = null, bool isHeardPhrase = false)
        {
            var joke = await _jokesApi.Get(topic, isHeardPhrase);
            return View(joke);
        }

        [HttpPost]
        [Route(nameof(CreateJoke))]
        public async Task<ActionResult> CreateJoke([FromForm] string joke, string? topic = null)
        {
            if (TryValidateModel(joke) is false)
            {
                return BadRequest();
            }
            try
            {
                var newJoke = new JokesDto() { Joke = joke, Topic = topic! };
                var added = await _jokesApi.Create(newJoke);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route(nameof(GetAllJokes))]
        public async Task<ActionResult> GetAllJokes()
        {
            return View(await _jokesApi.GetAllJokes());
        }

        [HttpPost]
        [Route(nameof(DeleteJoke))]
        public async Task DeleteJoke(int id)
        {
            var isDeleted = await _jokesApi.DeleteJoke(id);
            if (isDeleted is false)
            {
                throw new InvalidCastException($"There was an unexpected problem deleting the joke with Id = {id}");
            }
        }

        [HttpGet]
        [Route(nameof(Listen))]
        public async Task<IActionResult> Listen()
        {
            var basePath = _env.ContentRootPath;
            var modelPath = Path.Combine(basePath, nameof(Speech), nameof(Model), "vosk-model-en-us-0.15");
            var heardStatement = await Speech.Utility.Listen(modelPath);

            return RedirectToAction(nameof(GetJoke), new { topic = heardStatement, isHeardPhrase = true });
        }

        [HttpGet]
        [Route(nameof(Speak))]
        public async Task<IActionResult> Speak(string text)
        {
            AlsProjects.Speech.Utility.SpeakText(text);

            return await Task.FromResult(RedirectToAction("Index"));
        }
    }
}
