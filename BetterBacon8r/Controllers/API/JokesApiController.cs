using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlsProjects.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlsProjects.Controllers.API {

    [ApiController]
    [Route("api/[controller]")]
    public class JokesApiController : ControllerBase, IJokesApiController {
        private readonly Random _random;
        private readonly JokesDbContext _jokesDbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;

        public JokesApiController(IServiceProvider serviceProvider, IWebHostEnvironment env) {
            _random = new Random();
            _serviceProvider = serviceProvider;
            _jokesDbContext = _serviceProvider.GetRequiredService<JokesDbContext>();
            _env = env;
        }

        [HttpGet]
        [Route(nameof(Get))]
        public async Task<JokesDto> Get(string? topic = null, bool isHeardPhrase = false) {
            bool hasFoundJoke = false;
            var jokes = (await _jokesDbContext.Jokes
                .Select(e => e)
                .ToListAsync())[_random.Next(_jokesDbContext.Jokes.Count())];

            var jokeToReturn = new JokesDto() { Joke = jokes.Joke, Topic = jokes.Topic };

            if (isHeardPhrase && string.IsNullOrWhiteSpace(topic) is false)
                topic = isHeardPhrase ? CleanHeardPhrase(topic) : topic;

            if (string.IsNullOrWhiteSpace(topic) is false) {
                List<Jokes> maybeTopicalJoke = [];

                var topics = topic.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                maybeTopicalJoke = _jokesDbContext.Jokes
                    .AsEnumerable()
                    .Where(joke => topics.Any(word =>
                        joke.Topic.Contains(word, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                if (maybeTopicalJoke.Count != 0) {
                    hasFoundJoke = true;
                    var joke = maybeTopicalJoke[_random.Next(maybeTopicalJoke.Count())];
                    jokeToReturn = new JokesDto() { Topic = joke.Topic, Joke = joke.Joke };
                    if (isHeardPhrase)
                        jokeToReturn.VoiceInputDisposition = $"😎 I found you a *{jokeToReturn.Topic.ToLower()}* joke!";
                }
            }

            if (isHeardPhrase && hasFoundJoke is false && string.IsNullOrEmpty(topic) is false)
                jokeToReturn.VoiceInputDisposition = $"😳 I couldn't find a relevant joke about *{CleanHeardPhrase(topic)}*, but how about...";

            if (isHeardPhrase && string.IsNullOrEmpty(topic))
                jokeToReturn.VoiceInputDisposition = $"😳 I couldn't hear you real good, but how about...";

            return jokeToReturn;

            static string CleanHeardPhrase(string topic) {
                if (topic.Contains("about")) {
                    return topic.Substring(topic.IndexOf("about", StringComparison.InvariantCultureIgnoreCase) + "about".Count() + 1);
                }

                topic = topic?
                    .ToLower()
                    .Replace("tell ", " ")
                    .Replace(" me ", " ")
                    .Replace(" a ", " ")
                    .Replace(" joke", " ")
                    .Replace("how ", " ")
                    .Replace(" how ", " ")
                    .Replace(" how", " ")
                    .Replace("tommy", "")
                    .Trim() ?? "";
                return topic!;
            }
        }

        [HttpPost]
        public async Task<JokesDto> Create([FromBody] JokesDto joke) {
            var jokeToAdd = new Jokes() { Joke = joke.Joke, Topic = joke.Topic };
            _jokesDbContext.Jokes.Add(jokeToAdd);
            var added = await _jokesDbContext.SaveChangesAsync();

            if (added < 1)
                throw new ArgumentException("No joke was created (no joke)");

            return new JokesDto() { Topic = jokeToAdd.Topic, Joke = jokeToAdd.Joke };
        }

        [HttpGet]
        [Route(nameof(GetTopics))]
        public async Task<List<string>> GetTopics() {
            return await _jokesDbContext.Jokes
                .Select(j => char.ToUpper(j.Topic[0]) + j.Topic.Substring(1).ToLower())
                .Distinct()
                .ToListAsync();
        }

        [HttpGet]
        [Route(nameof(GetAllJokes))]
        public async Task<IEnumerable<Jokes>> GetAllJokes() {
            return await _jokesDbContext.Jokes.ToListAsync();
        }

        [HttpDelete]
        [Route(nameof(DeleteJoke))]
        public async Task<bool> DeleteJoke(int id) {
            var jokeToDelete = await _jokesDbContext.Jokes.FindAsync(id);
            if (jokeToDelete == null)
                throw new ArgumentException($"Joke Id not found: {id}");
            _jokesDbContext.Jokes.Remove(jokeToDelete);
            var deleted = await _jokesDbContext.SaveChangesAsync() > 0;
            return deleted;
        }
    }
}
