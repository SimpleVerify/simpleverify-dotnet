using System.Threading.Tasks;
using SimpleVerify.Models;
using SimpleVerify.Tests.Helpers;
using Xunit;

namespace SimpleVerify.Tests
{
    public class VerificationsResourceTests
    {
        [Fact]
        public async Task SendSmsVerification()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""abc-123"",
                    ""type"": ""sms"",
                    ""destination"": ""*******4567"",
                    ""status"": ""pending"",
                    ""expires_at"": ""2026-03-25T12:10:00+00:00"",
                    ""environment"": ""test"",
                    ""test"": { ""code"": ""482913"" }
                }
            }", System.Net.HttpStatusCode.Created);

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.SendAsync(new SendVerificationRequest
            {
                Type = "sms",
                Destination = "+15551234567",
            });

            Assert.Equal("abc-123", result.VerificationId);
            Assert.Equal("sms", result.Type);
            Assert.Equal("*******4567", result.Destination);
            Assert.Equal("pending", result.Status);
            Assert.Equal("test", result.Environment);
            Assert.NotNull(result.Test);
            Assert.Equal("482913", result.Test!.Code);
            Assert.Null(result.Test.Token);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(System.Net.Http.HttpMethod.Post, handler.LastRequest!.Method);
            Assert.EndsWith("/api/v1/verify/send", handler.LastRequest.RequestUri!.ToString());
        }

        [Fact]
        public async Task SendMagicLink()
        {
            var token = new string('a', 64);
            var handler = new MockHttpMessageHandler();
            handler.AddResponse($@"{{
                ""status"": ""success"",
                ""data"": {{
                    ""verification_id"": ""magic-456"",
                    ""type"": ""magic_link"",
                    ""destination"": ""u***@example.com"",
                    ""status"": ""pending"",
                    ""expires_at"": ""2026-03-25T12:25:00+00:00"",
                    ""environment"": ""test"",
                    ""test"": {{ ""token"": ""{token}"" }}
                }}
            }}", System.Net.HttpStatusCode.Created);

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.SendAsync(new SendVerificationRequest
            {
                Type = "magic_link",
                Destination = "user@example.com",
                RedirectUrl = "https://app.com/dashboard",
            });

            Assert.Equal("magic_link", result.Type);
            Assert.NotNull(result.Test);
            Assert.Equal(token, result.Test!.Token);
            Assert.Null(result.Test.Code);
        }

        [Fact]
        public async Task SendLiveHasNoTestData()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""live-789"",
                    ""type"": ""email"",
                    ""destination"": ""u***@example.com"",
                    ""status"": ""pending"",
                    ""expires_at"": ""2026-03-25T12:10:00+00:00"",
                    ""environment"": ""live""
                }
            }", System.Net.HttpStatusCode.Created);

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.SendAsync(new SendVerificationRequest
            {
                Type = "email",
                Destination = "user@example.com",
            });

            Assert.Equal("live", result.Environment);
            Assert.Null(result.Test);
        }

        [Fact]
        public async Task CheckValidCode()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""abc-123"",
                    ""valid"": true,
                    ""type"": ""sms"",
                    ""destination"": ""*******4567""
                }
            }");

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.CheckAsync("abc-123", "482913");

            Assert.True(result.Valid);
            Assert.Equal("sms", result.Type);
            Assert.Equal("*******4567", result.Destination);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(System.Net.Http.HttpMethod.Post, handler.LastRequest!.Method);
            Assert.EndsWith("/api/v1/verify/check", handler.LastRequest.RequestUri!.ToString());
        }

        [Fact]
        public async Task CheckInvalidCode()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""abc-123"",
                    ""valid"": false
                }
            }");

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.CheckAsync("abc-123", "000000");

            Assert.False(result.Valid);
            Assert.Null(result.Type);
            Assert.Null(result.Destination);
        }

        [Fact]
        public async Task GetVerificationStatus()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""abc-123"",
                    ""type"": ""sms"",
                    ""destination"": ""*******4567"",
                    ""status"": ""verified"",
                    ""expires_at"": ""2026-03-25T12:10:00+00:00"",
                    ""created_at"": ""2026-03-25T12:00:00+00:00""
                }
            }");

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.GetAsync("abc-123");

            Assert.Equal("verified", result.Status);
            Assert.Equal("2026-03-25T12:00:00+00:00", result.CreatedAt);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(System.Net.Http.HttpMethod.Get, handler.LastRequest!.Method);
            Assert.EndsWith("/api/v1/verify/abc-123", handler.LastRequest.RequestUri!.ToString());
        }

        [Fact]
        public async Task ExchangeMagicLink()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""success"",
                ""data"": {
                    ""verification_id"": ""magic-456"",
                    ""type"": ""magic_link"",
                    ""destination"": ""user@example.com"",
                    ""metadata"": { ""user_id"": 42 },
                    ""verified_at"": ""2026-03-25T12:05:00+00:00"",
                    ""environment"": ""test""
                }
            }");

            var client = TestClientFactory.Create(handler);
            var result = await client.Verifications.ExchangeAsync("magic-456", "exchange-code-123");

            Assert.Equal("magic_link", result.Type);
            Assert.Equal("user@example.com", result.Destination);
            Assert.Equal("test", result.Environment);
            Assert.NotNull(handler.LastRequest);
            Assert.Equal(System.Net.Http.HttpMethod.Post, handler.LastRequest!.Method);
            Assert.EndsWith("/api/v1/verify/exchange", handler.LastRequest.RequestUri!.ToString());
        }
    }
}
