using FeiertageApi.Utilities;
using System.Text;
using System.Text.Json;

namespace FeiertageApi.Tests.Utilities;

public class JsonConverterHelperTests
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("\"1\"", true)]
    [InlineData("\"0\"", false)]
    [InlineData("\"true\"", true)]
    [InlineData("\"false\"", false)]
    [InlineData("\"yes\"", true)]
    [InlineData("\"YES\"", true)]
    [InlineData("null", false)]
    public void ReadBoolish_ShouldParseCorrectly(string json, bool expected)
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        // Act
        var result = JsonConverterHelper.ReadBoolish(ref reader);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"1\"", true)]
    [InlineData("\"0\"", false)]
    [InlineData("\"true\"", true)]
    [InlineData("null", null)]
    public void ReadNullableBoolish_ShouldHandleNull(string json, bool? expected)
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        // Act
        var result = JsonConverterHelper.ReadNullableBoolish(ref reader);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WriteNullableBoolish_ShouldWriteNull()
    {
        // Arrange
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        JsonConverterHelper.WriteNullableBoolish(writer, null);
        writer.Flush();

        // Assert
        var json = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("null", json);
    }

    [Theory]
    [InlineData(true, "\"1\"")]
    [InlineData(false, "\"0\"")]
    public void WriteNullableBoolish_ShouldWriteBoolishString(bool value, string expected)
    {
        // Arrange
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        JsonConverterHelper.WriteNullableBoolish(writer, value);
        writer.Flush();

        // Assert
        var json = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal(expected, json);
    }

    [Theory]
    [InlineData("\"2024-12-25\"", 2024, 12, 25)]
    [InlineData("\"2025-01-01\"", 2025, 1, 1)]
    public void ParseDateOnly_ShouldParseCorrectly(string json, int year, int month, int day)
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        // Act
        var result = JsonConverterHelper.ParseDateOnly(ref reader);

        // Assert
        Assert.Equal(new DateOnly(year, month, day), result);
    }

    [Theory]
    [InlineData("\"invalid\"")]
    [InlineData("\"2024-25-12\"")]
    [InlineData("\"\"")]
    public void ParseDateOnly_ShouldThrow_WhenInvalidFormat(string json)
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes(json);

        // Act & Assert
        Assert.Throws<JsonException>(() =>
        {
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            JsonConverterHelper.ParseDateOnly(ref reader);
        });
    }
}