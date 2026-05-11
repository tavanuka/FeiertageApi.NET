using FeiertageApi.Clients;
using FeiertageApi.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeiertageApi.Tests.Clients;

public class FeiertageApiClientTests
{
    private const int KnownYear = 2024;

    private static FeiertageApiClient CreateClient()
        => new(NullLogger<FeiertageApiClient>.Instance);

    [Fact]
    public async Task GetPublicHolidays_WithYear_ReturnsHolidaysIncludingChristmasAndNewYear()
    {
        await using var client = CreateClient();

        var result = await client.GetPublicHolidays(KnownYear);

        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.NotEmpty(result.Holidays);
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 12, 25));
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 1, 1));
    }

    [Fact]
    public async Task GetPublicHolidays_WithBavaria_IncludesAssumptionDay()
    {
        await using var client = CreateClient();

        var result = await client.GetPublicHolidays(KnownYear, GermanState.Bavaria);

        Assert.Equal("success", result.Status);
        // Mariä Himmelfahrt (Aug 15) is observed in Bavaria but not in most other states,
        // making it a stable invariant for verifying state filtering.
        Assert.Contains(result.Holidays, h => h.Date == new DateOnly(KnownYear, 8, 15));
    }

    [Fact]
    public async Task GetPublicHolidays_WithMultipleStates_ReturnsHolidaysCoveringBothStates()
    {
        await using var client = CreateClient();

        var result = await client.GetPublicHolidays(
            KnownYear,
            [GermanState.Bavaria, GermanState.Berlin]);

        Assert.Equal("success", result.Status);
        var statesSeen = result.Holidays.SelectMany(h => h.States.Keys).ToHashSet();
        Assert.Contains(GermanState.Bavaria, statesSeen);
        Assert.Contains(GermanState.Berlin, statesSeen);
    }

    [Fact]
    public async Task StreamPublicHolidays_YieldsSameHolidaysAsGetPublicHolidays()
    {
        await using var client = CreateClient();

        var streamed = new List<Holiday>();
        await foreach (var holiday in client.StreamPublicHolidays(KnownYear, GermanState.Bavaria))
        {
            streamed.Add(holiday);
        }

        var nonStreamed = await client.GetPublicHolidays(KnownYear, GermanState.Bavaria);
        Assert.NotEmpty(streamed);
        Assert.Equal(nonStreamed.Holidays.Count, streamed.Count);
    }

    [Fact]
    public void Dispose_WhenUsingStandaloneConstructor_DoesNotThrow()
    {
        var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        var exception = Record.Exception(client.Dispose);

        Assert.Null(exception);
    }

    [Fact]
    public async Task DisposeAsync_WhenUsingStandaloneConstructor_DoesNotThrow()
    {
        var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        var exception = await Record.ExceptionAsync(async () => await client.DisposeAsync());

        Assert.Null(exception);
    }

    [Fact]
    public async Task GetPublicHolidays_AfterDispose_ThrowsObjectDisposedException()
    {
        var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);
        await client.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await client.GetPublicHolidays(KnownYear));
    }
}