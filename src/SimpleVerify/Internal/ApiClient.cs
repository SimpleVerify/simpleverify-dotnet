using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SimpleVerify.Exceptions;

namespace SimpleVerify.Internal
{
    internal class ApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public ApiClient(HttpClient httpClient, string apiKey, string baseUrl, TimeSpan timeout)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
            _httpClient.Timeout = timeout;
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var version = typeof(ApiClient).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "dev";
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"simpleverify-dotnet/{version}");
        }

        public async Task<T> RequestAsync<T>(HttpMethod method, string path, object? body = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            try
            {
                var request = new HttpRequestMessage(method, path);

                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body, JsonOptions);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ConnectionException("Failed to connect to SimpleVerify API: " + ex.Message);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ConnectionException("Request to SimpleVerify API timed out: " + ex.Message);
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            ApiResponse? apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);
            }
            catch (JsonException)
            {
                throw new Exceptions.ApiException(
                    "Invalid JSON response from SimpleVerify API",
                    (int)response.StatusCode);
            }

            if (apiResponse?.Status == "error")
            {
                ThrowException((int)response.StatusCode, apiResponse.Error);
            }

            if (apiResponse?.Data == null)
            {
                throw new Exceptions.ApiException(
                    "Missing data in SimpleVerify API response",
                    (int)response.StatusCode);
            }

            return JsonSerializer.Deserialize<T>(apiResponse.Data.Value.GetRawText())
                ?? throw new Exceptions.ApiException("Failed to deserialize response data", (int)response.StatusCode);
        }

        private static void ThrowException(int httpStatus, ApiError? error)
        {
            var message = error?.Message ?? "Unknown API error";
            var code = error?.Code;
            var details = error?.Details;

            throw httpStatus switch
            {
                401 => new AuthenticationException(message, httpStatus, code, details),
                404 => new NotFoundException(message, httpStatus, code, details),
                422 => new ValidationException(message, httpStatus, code, details),
                429 => new RateLimitException(message, httpStatus, code, details),
                _ => new Exceptions.ApiException(message, httpStatus, code, details),
            };
        }
    }
}
