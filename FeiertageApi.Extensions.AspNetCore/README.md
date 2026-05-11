# FeiertageApi.Extensions.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/FeiertageApi.Extensions.AspNetCore.svg?label=FeiertageApi.Extensions.AspNetCore)](https://www.nuget.org/packages/FeiertageApi.Extensions.AspNetCore/)

ASP.NET Core / dependency-injection integration for [FeiertageApi.NET](https://github.com/tavanuka/FeiertageApi.NET) — a strongly-typed .NET client for the [Feiertage API](https://feiertage-api.de/).

Installs the core [`FeiertageApi`](https://www.nuget.org/packages/FeiertageApi/) client transitively, so this package is all you need when using DI.

## Installation

```bash
dotnet add package FeiertageApi.Extensions.AspNetCore
```

## Usage

### Register the client

```csharp
using FeiertageApi.Extensions.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFeiertageApi();

var app = builder.Build();
```

`AddFeiertageApi()` registers `IFeiertageApiClient` against a named `HttpClient` and applies the standard resilience handler (retry + circuit breaker via `Microsoft.Extensions.Http.Resilience`).

### Inject and use

```csharp
using FeiertageApi.Clients;
using FeiertageApi.Models;

public class HolidayService
{
    private readonly IFeiertageApiClient _client;

    public HolidayService(IFeiertageApiClient client) => _client = client;

    public Task<HolidayResponse> GetBavarianHolidays() =>
        _client.GetPublicHolidays(2024, GermanState.Bavaria);
}
```

For the full API reference, streaming, error handling, and standalone (non-DI) usage, see the [main FeiertageApi.NET documentation](https://github.com/tavanuka/FeiertageApi.NET#readme).