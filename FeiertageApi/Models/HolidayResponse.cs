using FeiertageApi.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FeiertageApi.Models;

/// <summary>
/// Represents the response returned from the holiday API, including the status,
/// a collection of holidays, and an optional error message if applicable.
/// </summary>
/// <param name="Status">The status of the response, indicating success or failure.</param>
/// <param name="Holidays">A collection of holidays included in the response.</param>
/// <param name="ErrorMessage">An optional error message describing an issue with the response, if any.</param>
[JsonConverter(typeof(HolidayResponseJsonConverter))]
public sealed record HolidayResponse(string Status, IReadOnlyList<Holiday> Holidays, string? ErrorMessage = null);

/// <summary>
/// Represents a holiday with its associated details, including its date, name,
/// state-specific applicability, and additional attributes.
/// </summary>
/// <param name="Date">The date of the holiday.</param>
/// <param name="Name">The name of the holiday.</param>
/// <param name="AllStates">Indicates whether the holiday is applicable in all states.</param>
/// <param name="States">A dictionary indicating the applicability of the holiday in specific German states.</param>
/// <param name="Comment">Additional commentary or notes for the holiday, if any.</param>
/// <param name="Augsburg">Indicates whether the holiday is specific to Augsburg.</param>
/// <param name="Catholic">Indicates whether the holiday has a Catholic tradition or significance.</param>
public sealed record Holiday(
    DateOnly Date,
    string Name,
    bool AllStates,
    IReadOnlyDictionary<GermanState, bool> States,
    string Comment,
    bool? Augsburg,
    bool? Catholic
);
