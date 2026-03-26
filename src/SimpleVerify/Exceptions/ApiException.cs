using System.Collections.Generic;

namespace SimpleVerify.Exceptions
{
    public class ApiException : SimpleVerifyException
    {
        public ApiException(
            string message,
            int? httpStatus = null,
            string? errorCode = null,
            Dictionary<string, object>? details = null)
            : base(message, httpStatus, errorCode, details) { }
    }
}
