using System;
using System.Collections.Generic;

namespace SimpleVerify.Exceptions
{
    public class SimpleVerifyException : Exception
    {
        public int? HttpStatus { get; }
        public string? ErrorCode { get; }
        public Dictionary<string, object>? Details { get; }

        public SimpleVerifyException(
            string message,
            int? httpStatus = null,
            string? errorCode = null,
            Dictionary<string, object>? details = null)
            : base(message)
        {
            HttpStatus = httpStatus;
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
