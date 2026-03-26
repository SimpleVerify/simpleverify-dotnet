using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using SimpleVerify.Internal;

namespace SimpleVerify
{
    public class SimpleVerifyClient
    {
        private static readonly Regex ApiKeyPattern = new Regex(@"^vk_(test|live)_[0-9a-f]{64}$", RegexOptions.Compiled);

        private readonly ApiClient _apiClient;
        private VerificationsResource? _verifications;

        public VerificationsResource Verifications => _verifications ??= new VerificationsResource(_apiClient);

        public SimpleVerifyClient(string apiKey)
            : this(new SimpleVerifyOptions { ApiKey = apiKey })
        {
        }

        public SimpleVerifyClient(SimpleVerifyOptions options)
            : this(options, null)
        {
        }

        internal SimpleVerifyClient(SimpleVerifyOptions options, HttpMessageHandler? handler)
        {
            if (string.IsNullOrEmpty(options.ApiKey))
                throw new ArgumentException("API key is required.", nameof(options));

            if (!ApiKeyPattern.IsMatch(options.ApiKey))
                throw new ArgumentException(
                    "Invalid API key format. Expected vk_test_ or vk_live_ followed by 64 hex characters.",
                    nameof(options));

            var httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
            _apiClient = new ApiClient(httpClient, options.ApiKey, options.BaseUrl, options.Timeout);
        }
    }
}
