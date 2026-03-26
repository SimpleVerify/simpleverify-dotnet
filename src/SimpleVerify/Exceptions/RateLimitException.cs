using System.Collections.Generic;

namespace SimpleVerify.Exceptions
{
    public class RateLimitException : SimpleVerifyException
    {
        public int RetryAfterSeconds { get; }

        public RateLimitException(
            string message,
            int? httpStatus = 429,
            string? errorCode = null,
            Dictionary<string, object>? details = null)
            : base(message, httpStatus, errorCode, details)
        {
            if (details != null && details.TryGetValue("retry_after_seconds", out var value))
            {
                if (value is int intVal)
                    RetryAfterSeconds = intVal;
                else if (value is long longVal)
                    RetryAfterSeconds = (int)longVal;
                else if (int.TryParse(value?.ToString(), out var parsed))
                    RetryAfterSeconds = parsed;
            }
        }
    }
}
