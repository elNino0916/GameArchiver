using System.Text.Json.Serialization;

namespace GameArchiver.Models
{
    /// <summary>
    /// Represents a custom installation step to execute after extraction.
    /// </summary>
    public class CustomStep
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("Path")]
        public string Path { get; set; } = "";

        [JsonPropertyName("Description")]
        public string? Description { get; set; }
    }
}
