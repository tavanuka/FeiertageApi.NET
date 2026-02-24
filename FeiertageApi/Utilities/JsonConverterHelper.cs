using System;
using System.Globalization;
using System.Text.Json;

namespace FeiertageApi.Utilities;

internal static class JsonConverterHelper
{
    /// <summary>
    /// Reads a nullable "boolish" value from the JSON reader. A "boolish" value can be a JSON Boolean,
    /// a numeric value interpreted as a Boolean, a string representing a Boolean, or a null value.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> providing the JSON data to read
    /// the value from. The reader must be positioned at the value to be read.</param>
    /// <returns>
    /// A nullable Boolean value that represents the "boolish" value parsed from the JSON input,
    /// or null if the JSON token is of type <see cref="JsonTokenType.Null"/>.
    /// </returns>
    public static bool? ReadNullableBoolish(ref Utf8JsonReader reader)
        => reader.TokenType is JsonTokenType.Null
            ? null
            : ReadBoolish(ref reader);

    /// <summary>
    /// Writes a nullable "boolish" value to the JSON writer. A "boolish" value is represented as
    /// a "1" or "0" string for true or false, respectively, or as a JSON null if the value is null.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.
    /// This writer must be properly initialized and positioned for writing the value.</param>
    /// <param name="value">A nullable Boolean value to be written. The value is written as a
    /// "1" for true, "0" for false, or null if the value is null.</param>
    public static void WriteNullableBoolish(Utf8JsonWriter writer, bool? value)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value ? "1" : "0");
    }

    /// <summary>
    /// Reads a "boolish" value from the JSON reader. A "boolish" value can be a JSON Boolean,
    /// a numeric value interpreted as a Boolean, a string representing a Boolean, or a null value.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="Utf8JsonReader"/> providing the JSON data to read the value from.
    /// The reader must be positioned at the value to be read.
    /// </param>
    /// <returns>
    /// A Boolean value that represents the "boolish" value parsed from the JSON input.
    /// For JSON null tokens, the method returns false. For unsupported tokens, a <see cref="JsonException"/> is thrown.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token cannot be interpreted as a "boolish" value or when the token type is unsupported.
    /// </exception>
    public static bool ReadBoolish(ref Utf8JsonReader reader)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt32(out var n) ? n != 0 : throw new JsonException("Invalid numeric bool."),
            JsonTokenType.String => ParseBoolishString(reader.GetString()),
            JsonTokenType.Null => false,
            _ => throw new JsonException($"Unsupported token for boolish value: {reader.TokenType}.")
        };

    }

    /// <summary>
    /// Parses a JSON string token into a <see cref="DateOnly"/> instance. The string must be
    /// in the format "yyyy-MM-dd".
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at a JSON string token representing the date.</param>
    /// <returns>
    /// A <see cref="DateOnly"/> instance parsed from the JSON string.
    /// Throws a <see cref="JsonException"/> if the token is not a string, if it is empty,
    /// or if it does not match the expected format "yyyy-MM-dd".
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string, the string is null or empty,
    /// or the string format does not match "yyyy-MM-dd".
    /// </exception>
    public static DateOnly ParseDateOnly(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("date must be a string.");

        var s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s))
            throw new JsonException("date cannot be empty.");

        return DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d
            : throw new JsonException($"Invalid date format: '{s}'. Expected yyyy-MM-dd.");
    }

    /// <summary>
    /// Parses a string value to determine its "boolish" representation. A "boolish" string is a value
    /// that can be interpreted as a Boolean, such as "1", "true", or "yes".
    /// </summary>
    /// <param name="s">The input string to be evaluated. Can be null or contain whitespace.</param>
    /// <returns>
    /// A Boolean value representing the interpretation of the input string. Returns true for values
    /// like "1", "true", or "yes" (case-insensitive), and false for null, empty, or other non-"boolish" strings.
    /// </returns>
    private static bool ParseBoolishString(string? s)
    {
        if (s is null) return false;

        s = s.Trim();

        return s.Equals("1", StringComparison.OrdinalIgnoreCase)
               || s.Equals("true", StringComparison.OrdinalIgnoreCase)
               || s.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }
}