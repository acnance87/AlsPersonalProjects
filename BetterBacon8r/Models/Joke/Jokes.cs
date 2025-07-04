using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[ExcludeFromCodeCoverage]
public class Jokes {
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = null!;
    [JsonPropertyName("joke")]
    public string Joke { get; set; } = null!;
}
