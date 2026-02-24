using System.Collections.Generic;

namespace FeiertageApi.Models;

/// <summary>
/// Represents a request for retrieving holiday information.
/// </summary>
/// <param name="Years">
/// A collection of years for which holiday information is requested.
/// </param>
/// <param name="States">
/// A collection of German states for which holiday information is requested.
/// </param>
/// <param name="AllStates">
/// A boolean flag indicating whether to include only state-wide holidays. Defaults to false.
/// </param>
/// <param name="Catholic">
/// An optional boolean flag indicating whether to include Catholic holidays. Set to <c>false</c>, or leave Null to ignore this filter.
/// </param>
/// <param name="Augsburg">
/// An optional boolean flag indicating whether to include holidays specific to Augsburg. Set to <c>true</c> or Null to ignore this filter.
/// </param>
public record HolidayRequest(
    IEnumerable<int> Years,
    IEnumerable<GermanState> States,
    bool AllStates = false,
    bool? Catholic = null,
    bool? Augsburg = null);
