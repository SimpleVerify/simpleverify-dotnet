using System.Collections.Generic;

namespace SimpleVerify.Exceptions
{
    public class NotFoundException : SimpleVerifyException
    {
        public NotFoundException(
            string message,
            int? httpStatus = null,
            string? errorCode = null,
            Dictionary<string, object>? details = null)
            : base(message, httpStatus, errorCode, details) { }
    }
}
