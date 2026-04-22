using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleVerify.Internal;
using SimpleVerify.Models;

namespace SimpleVerify
{
    public class VerificationsResource
    {
        private readonly ApiClient _apiClient;

        internal VerificationsResource(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<Verification> SendAsync(SendVerificationRequest request, CancellationToken cancellationToken = default)
        {
            return _apiClient.RequestAsync<Verification>(HttpMethod.Post, "/v1/verify/send", request, cancellationToken);
        }

        public Task<VerificationCheck> CheckAsync(string verificationId, string code, CancellationToken cancellationToken = default)
        {
            var body = new { verification_id = verificationId, code };
            return _apiClient.RequestAsync<VerificationCheck>(HttpMethod.Post, "/v1/verify/check", body, cancellationToken);
        }

        public Task<Verification> GetAsync(string verificationId, CancellationToken cancellationToken = default)
        {
            return _apiClient.RequestAsync<Verification>(HttpMethod.Get, $"/v1/verify/{verificationId}", cancellationToken: cancellationToken);
        }

        public Task<MagicLinkExchange> ExchangeAsync(string verificationId, string exchangeCode, CancellationToken cancellationToken = default)
        {
            var body = new { verification_id = verificationId, exchange_code = exchangeCode };
            return _apiClient.RequestAsync<MagicLinkExchange>(HttpMethod.Post, "/v1/verify/exchange", body, cancellationToken);
        }
    }
}
