using System.Text.Json.Serialization;

namespace SimpleVerify.Models
{
    public class VerificationCheck
    {
        [JsonPropertyName("verification_id")]
        public string VerificationId { get; set; } = string.Empty;

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("destination")]
        public string? Destination { get; set; }
    }
}
