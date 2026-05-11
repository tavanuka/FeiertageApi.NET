using FeiertageApi.Models;
using System.Text.Json.Serialization;

namespace FeiertageApi.Converters;

/// <summary>
/// Source-generated JSON serializer metadata for the Feiertage API client. Using this
/// context (via <c>FeiertageJsonContext.Default.HolidayResponse</c>) instead of the
/// reflection-based <see cref="System.Text.Json.JsonSerializer"/> overloads keeps the
/// library trim- and AOT-friendly: no <c>RequiresUnreferencedCode</c> / <c>RequiresDynamicCode</c>
/// warnings, and downstream apps can publish with <c>PublishAot=true</c> without losing
/// holiday parsing.
/// </summary>
/// <remarks>
/// <see cref="HolidayResponse"/> is annotated with <see cref="JsonConverterAttribute"/> pointing at
/// <see cref="HolidayResponseJsonConverter"/>, so the generated metadata still delegates the
/// actual read/write to that custom converter. The benefit here is at the top-level entry point:
/// no reflection scan to discover the converter and no runtime <c>JsonTypeInfo</c> creation.
/// </remarks>
[JsonSerializable(typeof(HolidayResponse))]
internal partial class FeiertageJsonContext : JsonSerializerContext;
