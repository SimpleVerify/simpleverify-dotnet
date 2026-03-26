using System;

namespace SimpleVerify
{
    public class SimpleVerifyOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.simpleverify.io";
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
