using System;
using System.Net;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Exception thrown when the Feiertage API returns an error response.
/// </summary>
public sealed class FeiertageApiResponseException : FeiertageApiException
{
    /// <summary>
    /// Gets the error description returned by the API.
    /// </summary>
    public string? ErrorDescription { get; }

    /// <summary>
    /// Gets the status field from the API response.
    /// </summary>
    public string? ApiStatus { get; }

    public FeiertageApiResponseException(
        string message,
        string? apiStatus = null,
        string? errorDescription = null,
        HttpStatusCode? statusCode = null,
        string? responseContent = null,
        Uri? requestUri = null)
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
