using System.Text.Json;
using SimpleVerify.Models;
using Xunit;

namespace SimpleVerify.Tests
{
    public class ModelTests
    {
        [Fact]
        public void DeserializeVerification()
        {
            var json = @"{
                ""verification_id"": ""abc-123"",
                ""type"": ""sms"",
                ""destination"": ""*******4567"",
                ""status"": ""pending"",
                ""expires_at"": ""2026-03-25T12:10:00+00:00"",
                ""environment"": ""test"",
                ""created_at"": ""2026-03-25T12:00:00+00:00"",
                ""test"": { ""code"": ""123456"" }
            }";

            var v = JsonSerializer.Deserialize<Verification>(json)!;

            Assert.Equal("abc-123", v.VerificationId);
            Assert.Equal("sms", v.Type);
            Assert.Equal("*******4567", v.Destination);
            Assert.Equal("pending", v.Status);
            Assert.Equal("2026-03-25T12:10:00+00:00", v.ExpiresAt);
            Assert.Equal("test", v.Environment);
            Assert.Equal("2026-03-25T12:00:00+00:00", v.CreatedAt);
            Assert.NotNull(v.Test);
            Assert.Equal("123456", v.Test!.Code);
        }

        [Fact]
        public void DeserializeVerificationWithoutOptionalFields()
        {
            var json = @"{
                ""verification_id"": ""abc-123"",
                ""type"": ""email"",
                ""destination"": ""u***@example.com"",
                ""status"": ""verified""
            }";

            var v = JsonSerializer.Deserialize<Verification>(json)!;

            Assert.Null(v.ExpiresAt);
            Assert.Null(v.Environment);
            Assert.Null(v.CreatedAt);
            Assert.Null(v.Test);
        }

        [Fact]
        public void DeserializeVerificationCheckValid()
        {
            var json = @"{
                ""verification_id"": ""abc-123"",
                ""valid"": true,
                ""type"": ""sms"",
                ""destination"": ""*******4567""
            }";

            var vc = JsonSerializer.Deserialize<VerificationCheck>(json)!;

            Assert.Equal("abc-123", vc.VerificationId);
            Assert.True(vc.Valid);
            Assert.Equal("sms", vc.Type);
            Assert.Equal("*******4567", vc.Destination);
        }

        [Fact]
        public void DeserializeVerificationCheckInvalid()
        {
            var json = @"{
                ""verification_id"": ""abc-123"",
                ""valid"": false
            }";

            var vc = JsonSerializer.Deserialize<VerificationCheck>(json)!;

            Assert.False(vc.Valid);
            Assert.Null(vc.Type);
            Assert.Null(vc.Destination);
        }

        [Fact]
        public void DeserializeTestDataWithCode()
        {
            var json = @"{ ""code"": ""482913"" }";
            var td = JsonSerializer.Deserialize<TestData>(json)!;

            Assert.Equal("482913", td.Code);
            Assert.Null(td.Token);
        }

        [Fact]
        public void DeserializeTestDataWithToken()
        {
            var token = new string('a', 64);
            var json = $@"{{ ""token"": ""{token}"" }}";
            var td = JsonSerializer.Deserialize<TestData>(json)!;

            Assert.Null(td.Code);
            Assert.Equal(token, td.Token);
        }

        [Fact]
        public void SerializeSendRequest()
        {
            var request = new SendVerificationRequest
            {
                Type = "sms",
                Destination = "+15551234567",
            };

            var json = JsonSerializer.Serialize(request);

            Assert.Contains("\"type\":\"sms\"", json);
            Assert.Contains("\"destination\":\"+15551234567\"", json);
            Assert.DoesNotContain("redirect_url", json);
            Assert.DoesNotContain("metadata", json);
        }

        [Fact]
        public void SerializeSendRequestWithRedirectUrl()
        {
            var request = new SendVerificationRequest
            {
                Type = "magic_link",
                Destination = "user@example.com",
                RedirectUrl = "https://app.com/dashboard",
            };

            var json = JsonSerializer.Serialize(request);

            Assert.Contains("redirect_url", json);
            Assert.Contains("https://app.com/dashboard", json);
        }
    }
}
