using FeiertageApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FeiertageApi.Extensions;

/// <summary>
/// Extension methods for the <see cref="GermanState"/> enum.
/// </summary>
public static class GermanStateExtensions
{
    /// <summary>
    /// Converts a GermanState enum value to its corresponding API code (e.g., Bavaria -> "by").
    /// </summary>
    /// <param name="state">The German state enum value.</param>
    /// <returns>The two-letter state code used by the Feiertage API.</returns>
    /// <remarks>
    /// The mapping is hardcoded rather than reflected from the <c>[Description]</c> attributes
    /// on <see cref="GermanState"/> so that this code is AOT- and trimming-safe.
    /// </remarks>
    /// <example>
    /// <code>
    /// var code = GermanState.Bavaria.ToStateCode(); // Returns "by"
    /// </code>
    /// </example>
    public static string ToStateCode(this GermanState state) => state switch
    {
        GermanState.BadenWuerttemberg => "bw",
        GermanState.Bavaria => "by",
        GermanState.Berlin => "be",
        GermanState.Brandenburg => "bb",
        GermanState.Bremen => "hb",
        GermanState.Hamburg => "hh",
        GermanState.Hesse => "he",
        GermanState.MecklenburgVorpommern => "mv",
        GermanState.LowerSaxony => "ni",
        GermanState.NorthRhineWestphalia => "nw",
        GermanState.RhinelandPalatinate => "rp",
        GermanState.Saarland => "sl",
        GermanState.Saxony => "sn",
        GermanState.SaxonyAnhalt => "st",
        GermanState.SchleswigHolstein => "sh",
        GermanState.Thuringia => "th",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown GermanState value.")
    };

    /// <summary>
    /// Converts a collection of GermanState enum values to their corresponding API codes.
    /// </summary>
    /// <param name="states">Collection of German state enum values.</param>
    /// <returns>Collection of two-letter state codes.</returns>
    /// <example>
    /// <code>
    /// var codes = new[] { GermanState.Bavaria, GermanState.Berlin }.ToStateCodes();
    /// // Returns ["by", "be"]
    /// </code>
    /// </example>
    public static IEnumerable<string> ToStateCodes(this IEnumerable<GermanState> states) 
        => states.Select(s => s.ToStateCode());

    /// <summary>
    /// Parses a state code string to its corresponding GermanState enum value.
    /// </summary>
    /// <param name="stateCode">The two-letter state code (e.g., "by", "be").</param>
    /// <returns>The corresponding GermanState enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when the state code is invalid or not recognized.</exception>
    /// <example>
    /// <code>
    /// var state = GermanStateExtensions.FromStateCode("by"); // Returns GermanState.Bavaria
    /// </code>
    /// </example>
    public static GermanState FromStateCode(string stateCode)
    {
        if (string.IsNullOrWhiteSpace(stateCode))
            throw new ArgumentException("State code cannot be null or empty.", nameof(stateCode));

        var normalizedCode = stateCode.Trim().ToLowerInvariant();

        return Enum.GetValues<GermanState>()
            .Where(s => s.ToStateCode().Equals(normalizedCode, StringComparison.OrdinalIgnoreCase))
            .Select(s => (GermanState?)s)
            .FirstOrDefault()
            ?? throw new ArgumentException(
                $"Invalid state code: '{stateCode}'.",
                nameof(stateCode));
    }

    /// <summary>
    /// Tries to parse a state code string to its corresponding GermanState enum value.
    /// </summary>
    /// <param name="stateCode">The two-letter state code.</param>
    /// <param name="state">The resulting GermanState enum value if parsing succeeds.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (GermanStateExtensions.TryParseStateCode("by", out var state))
    /// {
    ///     Console.WriteLine(state); // GermanState.Bavaria
    /// }
    /// </code>
    /// </example>
    public static bool TryParseStateCode(string stateCode, out GermanState state)
    {
        state = default;

        if (string.IsNullOrWhiteSpace(stateCode))
            return false;

        try
        {
            state = FromStateCode(stateCode);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets all German states as an enumerable collection.
    /// </summary>
    /// <returns>All 16 German states.</returns>
    /// <example>
    /// <code>
    /// var allStates = GermanStateExtensions.GetAllStates();
    /// foreach (var state in allStates)
    /// {
    ///     Console.WriteLine($"{state}: {state.ToStateCode()}");
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<GermanState> GetAllStates() => Enum.GetValues<GermanState>();
}
