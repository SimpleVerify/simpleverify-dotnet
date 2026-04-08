using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleVerify.Models
{
    public class MagicLinkExchange
    {
        [JsonPropertyName("verification_id")]
        public string VerificationId { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = string.Empty;

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();

        [JsonPropertyName("verified_at")]
        public string? VerifiedAt { get; set; }

        [JsonPropertyName("environment")]
        public string? Environment { get; set; }
    }
}
