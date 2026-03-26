using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleVerify.Internal
{
    internal class ApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public JsonElement? Data { get; set; }

        [JsonPropertyName("error")]
        public ApiError? Error { get; set; }
    }
}
