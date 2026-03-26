using System.Threading.Tasks;
using SimpleVerify.Exceptions;
using SimpleVerify.Models;
using SimpleVerify.Tests.Helpers;
using Xunit;

namespace SimpleVerify.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public async Task AuthenticationException()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""error"",
                ""error"": {
                    ""code"": ""INVALID_API_KEY"",
                    ""message"": ""The provided API key is not valid.""
                }
            }", System.Net.HttpStatusCode.Unauthorized);

            var client = TestClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<Exceptions.AuthenticationException>(
                () => client.Verifications.SendAsync(new SendVerificationRequest
                {
                    Type = "sms",
                    Destination = "+15551234567",
                }));

            Assert.Equal(401, ex.HttpStatus);
            Assert.Equal("INVALID_API_KEY", ex.ErrorCode);
            Assert.Equal("The provided API key is not valid.", ex.Message);
        }

        [Fact]
        public async Task ValidationException()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""error"",
                ""error"": {
                    ""code"": ""VALIDATION_ERROR"",
                    ""message"": ""The given data was invalid."",
                    ""details"": {
                        ""destination"": [""Invalid phone number format.""]
                    }
                }
            }", System.Net.HttpStatusCode.UnprocessableEntity);

            var client = TestClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(
                () => client.Verifications.SendAsync(new SendVerificationRequest
                {
                    Type = "sms",
                    Destination = "not-a-phone",
                }));

            Assert.Equal(422, ex.HttpStatus);
            Assert.Equal("VALIDATION_ERROR", ex.ErrorCode);
            Assert.NotNull(ex.Details);
            Assert.True(ex.Details!.ContainsKey("destination"));
        }

        [Fact]
        public async Task RateLimitException()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""error"",
                ""error"": {
                    ""code"": ""RATE_LIMITED"",
                    ""message"": ""Too many verification attempts."",
                    ""details"": {
                        ""retry_after_seconds"": 25
                    }
                }
            }", (System.Net.HttpStatusCode)429);

            var client = TestClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<Exceptions.RateLimitException>(
                () => client.Verifications.SendAsync(new SendVerificationRequest
                {
                    Type = "sms",
                    Destination = "+15551234567",
                }));

            Assert.Equal(429, ex.HttpStatus);
            Assert.Equal("RATE_LIMITED", ex.ErrorCode);
            Assert.Equal(25, ex.RetryAfterSeconds);
        }

        [Fact]
        public async Task NotFoundException()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""error"",
                ""error"": {
                    ""code"": ""NOT_FOUND"",
                    ""message"": ""Resource not found.""
                }
            }", System.Net.HttpStatusCode.NotFound);

            var client = TestClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<Exceptions.NotFoundException>(
                () => client.Verifications.GetAsync("nonexistent-id"));

            Assert.Equal(404, ex.HttpStatus);
            Assert.Equal("NOT_FOUND", ex.ErrorCode);
        }

        [Fact]
        public async Task UnsupportedCountryIsValidationException()
        {
            var handler = new MockHttpMessageHandler();
            handler.AddResponse(@"{
                ""status"": ""error"",
                ""error"": {
                    ""code"": ""UNSUPPORTED_COUNTRY"",
                    ""message"": ""SMS is not currently supported for country: GB"",
                    ""details"": {
                        ""country_code"": ""GB"",
                        ""supported_countries"": [""US"", ""CA""]
                    }
                }
            }", System.Net.HttpStatusCode.UnprocessableEntity);

            var client = TestClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(
                () => client.Verifications.SendAsync(new SendVerificationRequest
                {
                    Type = "sms",
                    Destination = "+447911123456",
                }));

            Assert.Equal("UNSUPPORTED_COUNTRY", ex.ErrorCode);
            Assert.NotNull(ex.Details);
            Assert.True(ex.Details!.ContainsKey("country_code"));
        }
    }
}
