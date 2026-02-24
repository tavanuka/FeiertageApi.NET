using FeiertageApi.Clients;
using FeiertageApi.Exceptions;
using FeiertageApi.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace FeiertageApi.Tests.Clients;

public class FeiertageApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;

    public FeiertageApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://get.feiertage-api.de/")
        };
    }

    [Fact]
    public async Task GetPublicHolidays_WithYear_ShouldReturnHolidays()
    {
        // Arrange
        var responseJson = """
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
        """;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson)
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act
        var result = await client.GetPublicHolidays(2024);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.Single(result.Holidays);
        Assert.Equal("Christmas", result.Holidays[0].Name);
    }

    [Fact]
    public async Task GetPublicHolidays_WithYearAndState_ShouldIncludeStateInQuery()
    {
        // Arrange
        var responseJson = """
        {
            "status": "success",
            "feiertage": []
        }
        """;

        HttpRequestMessage? capturedRequest = null;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                };
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act
        await client.GetPublicHolidays(2024, GermanState.Bavaria);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Contains("states=by", capturedRequest!.RequestUri!.Query);
        Assert.Contains("years=2024", capturedRequest.RequestUri.Query);
    }

    [Fact]
    public async Task GetPublicHolidays_WithMultipleStates_ShouldIncludeAllStatesInQuery()
    {
        // Arrange
        var responseJson = """
        {
            "status": "success",
            "feiertage": []
        }
        """;

        HttpRequestMessage? capturedRequest = null;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                };
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act
        await client.GetPublicHolidays(2024, new[] { GermanState.Bavaria, GermanState.Berlin });

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Contains("states=by,be", capturedRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenApiReturnsError_ShouldThrowFeiertageApiResponseException()
    {
        // Arrange
        var responseJson = """
        {
            "status": "error",
            "error_description": "Invalid parameter",
            "feiertage": []
        }
        """;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson)
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FeiertageApiResponseException>(
            async () => await client.GetPublicHolidays(2024));
        Assert.Equal("Invalid parameter", ex.ErrorDescription);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenHttpError_ShouldThrowFeiertageApiHttpException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Server error")
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FeiertageApiHttpException>(
            async () => await client.GetPublicHolidays(2024));
        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
    }

    [Fact]
    public async Task GetPublicHolidays_WhenResponseIsNull_ShouldThrowFeiertageApiResponseException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act & Assert
        await Assert.ThrowsAsync<FeiertageApiResponseException>(
            async () => await client.GetPublicHolidays(2024));
    }

    [Fact]
    public async Task StreamPublicHolidays_ShouldYieldHolidays()
    {
        // Arrange
        var responseJson = """
        {
            "status": "success",
            "feiertage": [
                {
                    "date": "2024-01-01",
                    "fname": "New Year",
                    "all_states": "1",
                    "by": "1",
                    "comment": "",
                    "augsburg": null,
                    "katholisch": null
                },
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
        """;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson)
            });

        using var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);

        // Act
        var holidays = new List<Holiday>();
        await foreach (var holiday in client.StreamPublicHolidays(2024, GermanState.Bavaria))
        {
            holidays.Add(holiday);
        }

        // Assert
        Assert.Equal(2, holidays.Count);
        Assert.Equal("New Year", holidays[0].Name);
        Assert.Equal("Christmas", holidays[1].Name);
    }

    [Fact]
    public async Task Dispose_WhenUsingStandaloneConstructor_ShouldDisposeHttpClient()
    {
        // Arrange
        using var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        // Act & Assert - Should not throw
        client.Dispose();
    }

    [Fact]
    public async Task DisposeAsync_WhenUsingStandaloneConstructor_ShouldDisposeHttpClient()
    {
        // Arrange
        await using var client = new FeiertageApiClient(NullLogger<FeiertageApiClient>.Instance);

        // Act & Assert - Should not throw
        await client.DisposeAsync();
    }

    [Fact]
    public async Task GetPublicHolidays_AfterDispose_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);
        client.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await client.GetPublicHolidays(2024));
    }
}