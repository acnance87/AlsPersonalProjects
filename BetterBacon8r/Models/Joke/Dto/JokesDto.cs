using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AlsProjects.Model {

    [ExcludeFromCodeCoverage]
    public class JokesDto {
        [JsonPropertyName("topic")]
        [Required]
        public string Topic { get; set; } = null!;

        [JsonPropertyName("joke")]
        [Required]
        public string Joke { get; set; } = null!;

        public string VoiceInputDisposition { get; set; } = null!;
    }
}
