using FeiertageApi.Clients;
using FeiertageApi.Exceptions;
using FeiertageApi.Models;
using FeiertageApi.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace FeiertageApi.Tests.Clients;

public class FeiertageApiClientTests
{
    private const int KnownYear = 2024;

    [Fact]
    public async Task GetPublicHolidays_WithYear_ShouldReturnHolidays()
    {
        using var harness = TestHttpHarness.Returning("""
        {
            "status": "success",
            "feiertage": [
                {
                    "date": "2024-12-25",
                    "fname": "Christmas",
                    "all_states": "1",
                    "by": "1",
                    "comment": "",
                    "augsburg": null,
                    "katholisch": null
                }
            ]
        }
        """);

        var result = await harness.Client.GetPublicHolidays(KnownYear);

        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.Single(result.Holidays);
        Assert.Equal("Christmas", result.Holidays[0].Name);
    }

    [Fact]
    public async Task GetPublicHolidays_WithYearAndState_ShouldIncludeStateInQuery()
    {
        using var harness = TestHttpHarness.Returning("""{"status":"success","feiertage":[]}""");

        await harness.Client.GetPublicHolidays(KnownYear, GermanState.Bavaria);

        Assert.NotNull(harness.Handler.LastRequest);
        Assert.Contains("states=by", harness.Handler.LastRequest!.RequestUri!.Query);
        Assert.Contains("years=2024", harness.Handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task GetPublicHolidays_WithMultipleStates_ShouldIncludeAllStatesInQuery()
    {
        using var harness = TestHttpHarness.Returning("""{"status":"success","feiertage":[]}""");

        await harness.Client.GetPublicHolidays(KnownYear, [GermanState.Bavaria, GermanState.Berlin]);

        Assert.NotNull(harness.Handler.LastRequest);
        Assert.Contains("states=by,be", harness.Handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenApiReturnsError_ShouldThrowFeiertageApiResponseException()
    {
        using var harness = TestHttpHarness.Returning("""
        {
            "status": "error",
            "error_description": "Invalid parameter",
            "feiertage": []
        }
        """);

        var ex = await Assert.ThrowsAsync<FeiertageApiResponseException>(
            () => harness.Client.GetPublicHolidays(KnownYear));
        Assert.Equal("Invalid parameter", ex.ErrorDescription);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenHttpError_ShouldThrowFeiertageApiHttpException()
    {
        using var harness = TestHttpHarness.Returning("Server error", HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<FeiertageApiHttpException>(
            () => harness.Client.GetPublicHolidays(KnownYear));
        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenResponseIsNull_ShouldThrowFeiertageApiResponseException()
    {
        using var harness = TestHttpHarness.Returning("null");

        await Assert.ThrowsAsync<FeiertageApiResponseException>(
            () => harness.Client.GetPublicHolidays(KnownYear));
    }

    [Fact]
    public async Task StreamPublicHolidays_ShouldYieldHolidays()
    {
        using var harness = TestHttpHarness.Returning("""
        {
            "status": "success",
            "feiertage": [
                { "date": "2024-01-01", "fname": "New Year", "all_states": "1", "by": "1", "comment": "", "augsburg": null, "katholisch": null },
                { "date": "2024-12-25", "fname": "Christmas", "all_states": "1", "by": "1", "comment": "", "augsburg": null, "katholisch": null }
            ]
        }
        """);

        var holidays = new List<Holiday>();
        await foreach (var holiday in harness.Client.StreamPublicHolidays(KnownYear, GermanState.Bavaria))
            holidays.Add(holiday);

        Assert.Equal(2, holidays.Count);
        Assert.Equal("New Year", holidays[0].Name);
        Assert.Equal("Christmas", holidays[1].Name);
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
            () => client.GetPublicHolidays(KnownYear));
    }
}
