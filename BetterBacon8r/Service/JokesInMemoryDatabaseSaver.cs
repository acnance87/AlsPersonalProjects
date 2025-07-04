using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace AlsProjects.Service {
    public class JokesInMemoryDatabaseSaver {

        private readonly IWebHostEnvironment _env;

        public JokesInMemoryDatabaseSaver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task SaveJokesData(IEnumerable<Jokes> jokes) {
            var json = JsonSerializer.Serialize(jokes, new JsonSerializerOptions {
                WriteIndented = true
            });

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AlsProjects",
                "jokes.json");

            Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure folder exists
            await File.WriteAllTextAsync(path, json);
        }

        private string GetBackupFilePath() {
            var webRootPath = _env.ContentRootPath;
            return Path.Combine(webRootPath, "jokes.json");
        }
    }
}
