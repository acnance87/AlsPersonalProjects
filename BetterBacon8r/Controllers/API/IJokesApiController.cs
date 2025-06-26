using Microsoft.AspNetCore.Mvc;
using AlsProjects.Model;

namespace AlsProjects.Controllers.API {
    public interface IJokesApiController {
        Task<List<string>> GetTopics();
        Task<JokesDto> Get(string? topic = null, bool isHeardPhrase = false);
        Task<JokesDto> Create([FromBody] JokesDto joke);
        Task<IEnumerable<Jokes>> GetAllJokes();
        Task<bool> DeleteJoke(int id);
    }
}