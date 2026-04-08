# SimpleVerify .NET SDK

Official .NET client library for the [SimpleVerify](https://simpleverify.io) API. Send and verify SMS codes, email codes, and magic links with a few lines of code.

## Requirements

- .NET 6.0+ (or .NET Framework 4.6.1+ via netstandard2.0)

## Installation

```bash
dotnet add package SimpleVerify
```

Or via the NuGet Package Manager:

```
Install-Package SimpleVerify
```

## Quick Start

```csharp
using SimpleVerify;
using SimpleVerify.Models;

var client = new SimpleVerifyClient("vk_test_your_api_key_here");

// Send an SMS verification
var verification = await client.Verifications.SendAsync(new SendVerificationRequest
{
    Type = "sms",
    Destination = "+15551234567",
});

Console.WriteLine(verification.VerificationId); // "a1b2c3d4-..."
Console.WriteLine(verification.Status);          // "pending"

// Check the code the user entered
var result = await client.Verifications.CheckAsync(verification.VerificationId, "482913");

if (result.Valid)
{
    Console.WriteLine("Verified!");
}
```

## Usage

### Initialize the Client

```csharp
// With just an API key
var client = new SimpleVerifyClient("vk_test_...");

// With options
var client = new SimpleVerifyClient(new SimpleVerifyOptions
{
    ApiKey = "vk_test_...",
    BaseUrl = "https://api.simpleverify.io", // default
    Timeout = TimeSpan.FromSeconds(30),       // default
});
```

### Send a Verification

```csharp
// SMS
var verification = await client.Verifications.SendAsync(new SendVerificationRequest
{
    Type = "sms",
    Destination = "+15551234567",
});

// Email
var verification = await client.Verifications.SendAsync(new SendVerificationRequest
{
    Type = "email",
    Destination = "user@example.com",
});

// Magic link
var verification = await client.Verifications.SendAsync(new SendVerificationRequest
{
    Type = "magic_link",
    Destination = "user@example.com",
    RedirectUrl = "https://yourapp.com/dashboard",
    FailureRedirectUrl = "https://yourapp.com/auth/magic-link-result",
});

// With metadata
var verification = await client.Verifications.SendAsync(new SendVerificationRequest
{
    Type = "sms",
    Destination = "+15551234567",
    Metadata = new Dictionary<string, object> { ["user_id"] = 42 },
});
```

The response is a `Verification` object:

```csharp
verification.VerificationId // UUID
verification.Type           // "sms", "email", or "magic_link"
verification.Destination    // masked: "*******4567" or "u***@example.com"
verification.Status         // "pending"
verification.ExpiresAt      // ISO 8601 datetime
verification.Environment    // "test" or "live"
```

### Test Mode

When using a `vk_test_` API key, the response includes the code or token so you can complete the flow without real SMS/email delivery:

```csharp
verification.Test?.Code   // "482913" (SMS/email)
verification.Test?.Token  // 64-char string (magic link)
```

In live mode (`vk_live_` key), `verification.Test` is `null`.

If you set `FailureRedirectUrl` on a magic link, failed clicks redirect there with `status` (`invalid`, `expired`, or `already_used`) and `verification_id` query parameters.

Successful magic link clicks redirect with `status=verified`, `verification_id`, and a one-time `exchange_code`. Redeem that code from your backend:

```csharp
var exchange = await client.Verifications.ExchangeAsync(verificationId, exchangeCode);

exchange.Destination; // verified email address
exchange.Metadata;    // original metadata
```

### Check a Code

```csharp
var result = await client.Verifications.CheckAsync(verification.VerificationId, "482913");

result.Valid          // true or false
result.VerificationId // UUID
result.Type           // present when valid
result.Destination    // present when valid (masked)
```

An invalid code returns `Valid = false` (not an exception). Only check the `Valid` property.

### Get Verification Status

```csharp
var status = await client.Verifications.GetAsync(verification.VerificationId);

status.Status    // "pending", "verified", or "expired"
status.CreatedAt // ISO 8601 datetime
```

## Error Handling

All API errors throw specific exceptions extending `SimpleVerifyException`:

```csharp
using SimpleVerify.Exceptions;

try
{
    await client.Verifications.SendAsync(request);
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry in {ex.RetryAfterSeconds} seconds.");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation error: {ex.ErrorCode}");
    // ex.Details contains field-level errors
}
catch (AuthenticationException ex)
{
    Console.WriteLine($"Bad API key: {ex.ErrorCode}");
}
catch (NotFoundException ex)
{
    Console.WriteLine("Verification not found.");
}
catch (SimpleVerifyException ex)
{
    // Catch-all for any API error
    Console.WriteLine($"Status: {ex.HttpStatus}, Code: {ex.ErrorCode}, Message: {ex.Message}");
}
```

| HTTP Status | Exception |
|-------------|-----------|
| 401 | `AuthenticationException` |
| 404 | `NotFoundException` |
| 422 | `ValidationException` |
| 429 | `RateLimitException` |
| Other | `ApiException` |
| Network failure | `ConnectionException` |

## Cancellation

All async methods accept a `CancellationToken`:

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var verification = await client.Verifications.SendAsync(request, cts.Token);
```

## License

MIT
