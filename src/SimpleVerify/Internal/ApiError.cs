using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleVerify.Internal
{
    internal class ApiError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("details")]
        public Dictionary<string, object>? Details { get; set; }
    }
}
