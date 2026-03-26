using System.Text.Json.Serialization;

namespace SimpleVerify.Models
{
    public class TestData
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}
