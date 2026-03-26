namespace SimpleVerify.Tests.Helpers
{
    internal static class TestClientFactory
    {
        public const string ValidKey = "vk_test_a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2";

        public static SimpleVerifyClient Create(MockHttpMessageHandler handler)
        {
            var options = new SimpleVerifyOptions { ApiKey = ValidKey };
            return new SimpleVerifyClient(options, handler);
        }
    }
}
