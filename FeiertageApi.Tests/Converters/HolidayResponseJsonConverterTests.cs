using FeiertageApi.Models;
using System.Text.Json;

namespace FeiertageApi.Tests.Converters;

public class HolidayResponseJsonConverterTests
{
    [Fact]
    public void Deserialize_ShouldParseSuccessResponse()
    {
        // Arrange
        var json = """
        {
            "status": "success",
            "feiertage": [
                {
                    "date": "2024-12-25",
                    "fname": "1. Weihnachtstag",
                    "all_states": "1",
                    "by": "1",
                    "be": "1",
                    "comment": "",
                    "augsburg": null,
                    "katholisch": null
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<HolidayResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.Null(result.ErrorMessage);
        Assert.Single(result.Holidays);

        var holiday = result.Holidays[0];
        Assert.Equal(new DateOnly(2024, 12, 25), holiday.Date);
        Assert.Equal("1. Weihnachtstag", holiday.Name);
        Assert.True(holiday.AllStates);
        Assert.True(holiday.States.ContainsKey(GermanState.Bavaria));
        Assert.True(holiday.States[GermanState.Bavaria]);
        Assert.True(holiday.States.ContainsKey(GermanState.Berlin));
        Assert.Empty(holiday.Comment);
    }

    [Fact]
    public void Deserialize_ShouldParseErrorResponse()
    {
        // Arrange
        var json = """
        {
            "status": "error",
            "error_description": "Invalid parameter",
            "feiertage": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<HolidayResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("error", result.Status);
        Assert.Equal("Invalid parameter", result.ErrorMessage);
        Assert.Empty(result.Holidays);
    }

    [Fact]
    public void Deserialize_ShouldHandleMultipleHolidays()
    {
        // Arrange
        var json = """
        {
            "status": "success",
            "feiertage": [
                {
                    "date": "2024-01-01",
                    "fname": "Neujahr",
                    "all_states": "1",
                    "by": "1",
                    "comment": "Test",
                    "augsburg": "0",
                    "katholisch": "0"
                },
                {
                    "date": "2024-12-25",
                    "fname": "Weihnachtstag",
                    "all_states": "0",
                    "by": "1",
                    "be": "0",
                    "comment": "",
                    "augsburg": null,
                    "katholisch": "1"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<HolidayResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Holidays.Count);
        Assert.Equal(new DateOnly(2024, 1, 1), result.Holidays[0].Date);
        Assert.Equal(new DateOnly(2024, 12, 25), result.Holidays[1].Date);
        Assert.True(result.Holidays[1].Catholic);
    }

    [Fact]
    public void Deserialize_ShouldHandleUnknownStateCode()
    {
        // Arrange
        var json = """
        {
            "status": "success",
            "feiertage": [
                {
                    "date": "2024-12-25",
                    "fname": "Christmas",
                    "all_states": "1",
                    "by": "1",
                    "unknown_state": "1",
                    "comment": "",
                    "augsburg": null,
                    "katholisch": null
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<HolidayResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Holidays[0].States.ContainsKey(GermanState.Bavaria));
        Assert.False(result.Holidays[0].States.ContainsKey((GermanState)999)); // Unknown state ignored
    }

    [Fact]
    public void Serialize_ShouldProduceCorrectJson()
    {
        // Arrange
        var response = new HolidayResponse(
            "success",
            new List<Holiday>
            {
                new Holiday(
                    new DateOnly(2024, 12, 25),
                    "Christmas",
                    true,
                    new Dictionary<GermanState, bool>
                    {
                        { GermanState.Bavaria, true },
                        { GermanState.Berlin, false }
                    },
                    "Test comment",
                    null,
                    true
                )
            },
            null
        );

        // Act
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<HolidayResponse>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("success", deserialized.Status);
        Assert.Single(deserialized.Holidays);
        Assert.Equal("Christmas", deserialized.Holidays[0].Name);
        Assert.True(deserialized.Holidays[0].States[GermanState.Bavaria]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("[]")]
    [InlineData("{\"status\": 123}")]
    public void Deserialize_ShouldThrow_WhenInvalidJson(string json)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HolidayResponse>(json));
    }
}