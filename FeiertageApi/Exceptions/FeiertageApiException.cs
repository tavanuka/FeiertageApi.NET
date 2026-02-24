using System;
using System.Net;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Base exception type for all Feiertage API related errors.
/// </summary>
public class FeiertageApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code of the response, if available.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the raw response content, if available.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the request URI that caused the exception, if available.
    /// </summary>
    public Uri? RequestUri { get; }

    public FeiertageApiException(string message)
        : base(message)
    {
    }

    public FeiertageApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public FeiertageApiException(
        string message,
        HttpStatusCode? statusCode = null,
        string? responseContent = null,
        Uri? requestUri = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        RequestUri = requestUri;
    }
}
