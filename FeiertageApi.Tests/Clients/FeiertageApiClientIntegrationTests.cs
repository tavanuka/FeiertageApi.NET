using FeiertageApi.Clients;
using FeiertageApi.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeiertageApi.Tests.Clients;

/// <summary>
/// Live-API smoke tests. Hit the real Feiertage API to catch contract drift
/// (URL, response shape, state-code keys) that unit tests can't see.
/// Opted-out by default in CI via <c>--filter "Category!=Integration"</c> because
/// the public API has a 100 req/hour rate limit shared across all callers.
/// </summary>
[Trait("Category", "Integration")]
public class FeiertageApiClientIntegrationTests
{
    private const int KnownYear = 2024;

    [Fact]
    public async Task GetPublicHolidays_WithYear_ReturnsHolidaysIncludingChristmasAndNewYear()
    {
        await using var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        var result = await client.GetPublicHolidays(KnownYear);

        Assert.Equal("success", result.Status);
        Assert.NotEmpty(result.Holidays);
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 12, 25));
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 1, 1));
    }

    [Fact]
    public async Task GetPublicHolidays_WithBavaria_IncludesAssumptionDay()
    {
        await using var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        var result = await client.GetPublicHolidays(KnownYear, GermanState.Bavaria);

        Assert.Equal("success", result.Status);
        // Mariä Himmelfahrt (Aug 15) is observed in Bavaria — a stable real-world invariant
        // useful for catching state-filter regressions against the live API.
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 8, 15));
    }
}