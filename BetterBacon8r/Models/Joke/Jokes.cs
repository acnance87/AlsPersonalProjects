using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Jokes {
    public int Id { get; set; }
    public string Topic { get; set; } = null!;
    public string Joke { get; set; } = null!;
}
