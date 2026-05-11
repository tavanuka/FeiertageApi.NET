using System;
using System.Net;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Exception thrown when the Feiertage API returns a structurally-valid HTTP response that
/// the client cannot accept — either the body was unparseable / null, or the API itself
/// returned an <c>"error"</c> status field in an otherwise-200 response.
/// </summary>
public sealed class FeiertageApiResponseException : FeiertageApiException
{
    /// <summary>
    /// Gets the error description returned by the API, if it produced one.
    /// </summary>
    public string? ErrorDescription { get; }

    /// <summary>
    /// Gets the <c>status</c> field from the API response (e.g. <c>"error"</c>),
    /// if one was returned.
    /// </summary>
    public string? ApiStatus { get; }

    /// <summary>
    /// Initializes a new instance for a response the client could not accept — typically
    /// an empty body, a body that fails JSON parsing, or a successful HTTP response whose
    /// shape doesn't match the expected schema.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code of the response (usually 200).</param>
    /// <param name="responseContent">The raw response body.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    internal FeiertageApiResponseException(
        string message,
        HttpStatusCode? statusCode,
        string? responseContent,
        Uri? requestUri)
        : base(message, statusCode, responseContent, requestUri)
    {
    }

    /// <summary>
    /// Initializes a new instance for an API-level error — the HTTP response succeeded but
    /// the API returned a <c>status: "error"</c> field with an optional description.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="apiStatus">The <c>status</c> field returned by the API.</param>
    /// <param name="errorDescription">The <c>error_description</c> field returned by the API, if present.</param>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    /// <param name="responseContent">The raw response body.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    internal FeiertageApiResponseException(
        string message,
        string apiStatus,
        string? errorDescription,
        HttpStatusCode? statusCode,
        string? responseContent,
        Uri? requestUri)
        : base(message, statusCode, responseContent, requestUri)
    {
        ApiStatus = apiStatus;
        ErrorDescription = errorDescription;
    }

    public override string ToString()
    {
        var baseString = base.ToString();

        if (!string.IsNullOrEmpty(ErrorDescription))
            baseString += $"{Environment.NewLine}API Error: {ErrorDescription}";

        if (RequestUri != null)
            baseString += $"{Environment.NewLine}Request URI: {RequestUri}";

        return baseString;
    }
}