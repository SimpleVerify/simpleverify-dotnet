using System;
using Xunit;

namespace SimpleVerify.Tests
{
    public class SimpleVerifyClientTests
    {
        private const string ValidKey = "vk_test_a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2";

        [Fact]
        public void AcceptsValidTestKey()
        {
            var client = new SimpleVerifyClient(ValidKey);
            Assert.NotNull(client);
            Assert.NotNull(client.Verifications);
        }

        [Fact]
        public void AcceptsValidLiveKey()
        {
            var key = "vk_live_" + new string('a', 64);
            var client = new SimpleVerifyClient(key);
            Assert.NotNull(client);
        }

        [Fact]
        public void AcceptsOptions()
        {
            var client = new SimpleVerifyClient(new SimpleVerifyOptions
            {
                ApiKey = ValidKey,
                BaseUrl = "https://custom.api.com",
                Timeout = TimeSpan.FromSeconds(60),
            });
            Assert.NotNull(client);
        }

        [Fact]
        public void RejectsEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SimpleVerifyClient(""));
            Assert.Contains("API key is required", ex.Message);
        }

        [Fact]
        public void RejectsMissingKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SimpleVerifyClient(new SimpleVerifyOptions()));
            Assert.Contains("API key is required", ex.Message);
        }

        [Fact]
        public void RejectsInvalidPrefix()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SimpleVerifyClient("vk_bad_" + new string('a', 64)));
            Assert.Contains("Invalid API key format", ex.Message);
        }

        [Fact]
        public void RejectsShortKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SimpleVerifyClient("vk_test_tooshort"));
            Assert.Contains("Invalid API key format", ex.Message);
        }

        [Fact]
        public void VerificationsReturnsSameInstance()
        {
            var client = new SimpleVerifyClient(ValidKey);
            Assert.Same(client.Verifications, client.Verifications);
        }
    }
}
