using FeiertageApi.Extensions;
using FeiertageApi.Models;
using FeiertageApi.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeiertageApi.Converters;

public sealed class HolidayResponseJsonConverter : JsonConverter<HolidayResponse>
{
    /// <summary>
    /// Represents a collection of property names that are recognized as top-level fields in a holiday object.
    /// This set is used to differentiate standard properties from unknown properties when deserializing JSON data.
    /// The fields in this set include common holiday attributes such as the date of the holiday,
    /// its name, its applicability across states, and additional relevant information.
    /// </summary>
    private static readonly HashSet<string> KnownHolidayFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "date",
        "fname",
        "all_states",
        "comment",
        "augsburg",
        "katholisch"
    };

    public override HolidayResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for HolidayResponse.");

        string? status = null;
        string? errorDescription = null;
        List<Holiday>? holidays = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in HolidayResponse.");

            var propName = reader.GetString();
            reader.Read();

            switch (propName)
            {
                case "status":
                    status = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new JsonException("status must be a string.");
                    break;
                case "feiertage":
                    holidays = ReadHolidaysArray(ref reader);
                    break;
                case "error_description" when status == "error":
                    errorDescription = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new JsonException("error_description must be a string.");
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new HolidayResponse(
            Status: status ?? string.Empty,
            Holidays: holidays ?? [],
            ErrorMessage: errorDescription);
    }

    public override void Write(Utf8JsonWriter writer, HolidayResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("status", value.Status);

        writer.WritePropertyName("feiertage");
        writer.WriteStartArray();

        foreach (var h in value.Holidays)
        {
            writer.WriteStartObject();

            writer.WriteString("date", h.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            writer.WriteString("fname", h.Name);
            writer.WriteString("all_states", h.AllStates ? "1" : "0");

            // State flags as individual properties (bw/by/...) in the API format
            foreach (var kvp in h.States)
            {
                var stateCode = kvp.Key.ToStateCode();
                writer.WriteString(stateCode, kvp.Value ? "1" : "0");
            }

            writer.WriteString("comment", h.Comment);

            writer.WritePropertyName("augsburg");
            JsonConverterHelper.WriteNullableBoolish(writer, h.Augsburg);

            writer.WritePropertyName("katholisch");
            JsonConverterHelper.WriteNullableBoolish(writer, h.Catholic);

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    /// <summary>
    /// Reads an array of holiday objects from the JSON data and converts them into a list of <see cref="Holiday"/> objects.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> instance pointing to the JSON data containing the holiday array.</param>
    /// <returns>A list of <see cref="Holiday"/> objects parsed from the JSON data.</returns>
    /// <exception cref="JsonException">Thrown when the JSON data is not in the expected format, such as if the array is not properly structured.</exception>
    private static List<Holiday> ReadHolidaysArray(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("feiertage must be an array.");

        var list = new List<Holiday>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            list.Add(ReadHolidayObject(ref reader));
        }

        return list;
    }

    /// <summary>
    /// Reads a holiday object from the JSON data and converts it into a <see cref="Holiday"/> instance.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> instance pointing to the JSON data containing the holiday object.</param>
    /// <returns>A <see cref="Holiday"/> instance parsed from the JSON object in the input data.</returns>
    /// <exception cref="JsonException">Thrown when the JSON data does not contain a properly structured holiday object or when property values are of unexpected types.</exception>
    private static Holiday ReadHolidayObject(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for holiday.");

        DateOnly date = default;
        var name = string.Empty;
        var allStates = false;
        var states = new Dictionary<GermanState, bool>();
        var comment = string.Empty;
        bool? augsburg = null;
        bool? catholic = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in holiday object.");

            var propName = reader.GetString();
            reader.Read();

            if (propName is null)
            {
                reader.Skip();
                continue;
            }

            switch (propName)
            {
                case "date":
                    date = JsonConverterHelper.ParseDateOnly(ref reader);
                    break;

                case "fname":
                    name = reader.TokenType == JsonTokenType.String 
                        ? reader.GetString() ?? string.Empty 
                        : throw new JsonException("fname must be a string.");
                    break;

                case "all_states":
                    allStates = JsonConverterHelper.ReadBoolish(ref reader);
                    break;

                case "comment":
                    comment = reader.TokenType switch
                    {
                        JsonTokenType.String => reader.GetString() ?? string.Empty,
                        JsonTokenType.Null => string.Empty,
                        _ => throw new JsonException("comment must be a string or null.")
                    };
                    break;

                case "augsburg":
                    augsburg = JsonConverterHelper.ReadNullableBoolish(ref reader);
                    break;

                case "katholisch":
                    catholic = JsonConverterHelper.ReadNullableBoolish(ref reader);
                    break;

                default:
                    // Any unknown property in holiday object is treated as a "state flag" (bw/by/...)
                    // but we guard against known top-level fields.
                    if (!KnownHolidayFields.Contains(propName))
                    {
                        // Try to parse the state code to a GermanState enum
                        if (GermanStateExtensions.TryParseStateCode(propName, out var state))
                        {
                            states[state] = JsonConverterHelper.ReadBoolish(ref reader);
                        }
                        else
                        {
                            // Unknown state code, skip it
                            reader.Skip();
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                    break;
            }
        }

        return new Holiday(
            Date: date,
            Name: name,
            AllStates: allStates,
            States: states,
            Comment: comment,
            Augsburg: augsburg,
            Catholic: catholic);
    }
}