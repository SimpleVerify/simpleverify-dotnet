using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleVerify.Tests.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<(string Body, HttpStatusCode StatusCode)> _responses = new();
        private readonly List<HttpRequestMessage> _requests = new();

        public IReadOnlyList<HttpRequestMessage> Requests => _requests;
        public HttpRequestMessage? LastRequest => _requests.Count > 0 ? _requests[_requests.Count - 1] : null;

        public MockHttpMessageHandler AddResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responses.Enqueue((json, statusCode));
            return this;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requests.Add(request);

            var (body, statusCode) = _responses.Count > 0
                ? _responses.Dequeue()
                : ("{}", HttpStatusCode.OK);

            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            return Task.FromResult(response);
        }
    }
}
