# FeiertageApi.NET

A modern .NET library for accessing German public holidays through the [Feiertage API](https://feiertage-api.de/).

## Features

- **Simple & Intuitive API** - Easy-to-use methods for querying German public holidays
- **Async/Await Support** - Fully asynchronous with `Task` and `IAsyncEnumerable`
- **Streaming Support** - Memory-efficient streaming for large datasets
- **Resilient HTTP Client** - Built-in retry policies and circuit breakers
- **Structured Logging** - Integrated with `Microsoft.Extensions.Logging`
- **Rich Exception Handling** - Detailed error information with custom exception types
- **Dependency Injection** - First-class support for .NET DI
- **Disposable Pattern** - Supports both IDisposable and IAsyncDisposable for standalone usage
- **Strongly-Typed States** - Type-safe `GermanState` enum with IntelliSense support

## Installation

```bash
dotnet add package FeiertageApi.NET
```

## Quick Start

### Option 1: Standalone Usage (Without DI)

Perfect for console apps, scripts, or simple applications:

```csharp
using FeiertageApi.Clients;
using FeiertageApi.Models;

// Using statement ensures proper disposal
using var client = new FeiertageApiClient();
var holidays = await client.GetPublicHolidays(2024, GermanState.Bavaria);

foreach (var holiday in holidays.Holidays)
{
    Console.WriteLine($"{holiday.Date:yyyy-MM-dd}: {holiday.Name}");
}

// Or with async disposal
await using var client2 = new FeiertageApiClient();
var response = await client2.GetPublicHolidays(2025, GermanState.Berlin);
```

### Option 2: Dependency Injection

Recommended for ASP.NET Core and larger applications:

#### 1. Register the Service

```csharp
using FeiertageApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add FeiertageApi client with resilience handling
builder.Services.AddFeiertageApi();

var app = builder.Build();
```

#### 2. Inject and Use the Client

```csharp
using FeiertageApi.Clients;
using FeiertageApi.Models;

public class HolidayService
{
    private readonly IFeiertageApiClient _client;

    public HolidayService(IFeiertageApiClient client)
    {
        _client = client;
    }

    public async Task<HolidayResponse> GetBavarianHolidays()
    {
        // Get holidays for Bavaria in 2024 - strongly-typed!
        return await _client.GetPublicHolidays(2024, GermanState.Bavaria);
    }
}
```

## Usage Examples

```csharp
using FeiertageApi.Models;

// Single state - type-safe with IntelliSense
var holidays = await client.GetPublicHolidays(2024, GermanState.Bavaria);

// Multiple states
var holidays2 = await client.GetPublicHolidays(
    year: 2024,
    states: new[] { GermanState.Bavaria, GermanState.Berlin }
);

// Multiple years and states
var holidays3 = await client.GetPublicHolidays(
    years: new[] { 2024, 2025 },
    states: new[] { GermanState.Bavaria, GermanState.Berlin, GermanState.Hamburg }
);
```

### Get Holidays for a Specific Year

```csharp
// Get all holidays for 2024
var response = await client.GetPublicHolidays(2024);

foreach (var holiday in response.Holidays)
{
    Console.WriteLine($"{holiday.Date:yyyy-MM-dd}: {holiday.Name}");
}
```

### Filter by Catholic Holidays

```csharp
// Get only Catholic holidays for Bavaria in 2024
var response = await client.GetPublicHolidays(
    year: 2024,
    state: GermanState.Bavaria,
    catholic: true
);
```

### Get Holidays Across Multiple Years

```csharp
// Get holidays for 2024, 2025, and 2026
var response = await client.GetPublicHolidays(
    years: new[] { 2024, 2025, 2026 },
    states: new[] { GermanState.Bavaria }
);
```

### Stream Holidays (Memory Efficient)

```csharp
// Stream holidays one at a time - great for large datasets
await foreach (var holiday in client.StreamPublicHolidays(2024, GermanState.Bavaria))
{
    Console.WriteLine($"{holiday.Date:yyyy-MM-dd}: {holiday.Name}");

    // Process each holiday without loading all into memory
    await ProcessHolidayAsync(holiday);
}

// Stream multiple states
await foreach (var holiday in client.StreamPublicHolidays(
    new[] { 2024, 2025 },
    new[] { GermanState.Bavaria, GermanState.Berlin }))
{
    Console.WriteLine($"{holiday.Date}: {holiday.Name}");
}
```

### Using HolidayRequest Object

```csharp
var request = new HolidayRequest(
    Years: new[] { 2024 },
    States: new[] { GermanState.Bavaria, GermanState.BadenWuerttemberg },
    AllStates: false,
    Catholic: true,
    Augsburg: null
);

var response = await client.GetPublicHolidays(request);
```

## Error Handling

The library provides detailed exception types for different error scenarios:

```csharp
using FeiertageApi.Exceptions;

try
{
    var response = await client.GetPublicHolidays(2024, GermanState.Bavaria);
}
catch (FeiertageApiHttpException ex)
{
    // HTTP communication error (network, timeout, etc.)
    Console.WriteLine($"HTTP Error: {ex.StatusCode}");
    Console.WriteLine($"Request URI: {ex.RequestUri}");
}
catch (FeiertageApiResponseException ex)
{
    // API returned an error response
    Console.WriteLine($"API Error: {ex.ErrorDescription}");
    Console.WriteLine($"API Status: {ex.ApiStatus}");
}
catch (FeiertageApiException ex)
{
    // Base exception for all API errors
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Logging

The library integrates with `Microsoft.Extensions.Logging`:

```csharp
// Configure logging in your application
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
```

## Requirements

- .NET 10.0 or later
- Internet connection to access the Feiertage API

## API Reference

Full API documentation is available at [https://feiertage-api.de/](https://feiertage-api.de/)

## License

This library is provided as-is. The Feiertage API is provided by [feiertage-api.de](https://feiertage-api.de/).

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Support

For issues with this library, please open an issue on GitHub.

For questions about the Feiertage API itself, visit [https://feiertage-api.de/](https://feiertage-api.de/).
