using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Exception thrown when an HTTP-level error occurs communicating with the Feiertage API —
/// either the API returned a non-success status code, or a transport-level exception was
/// raised before/while reading the response.
/// </summary>
public sealed class FeiertageApiHttpException : FeiertageApiException
{
    /// <summary>
    /// Initializes a new instance for a response with a non-success status code.
    /// Use when the request completed and a response was received, but the status indicates
    /// the request was unsuccessful (4xx / 5xx).
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="responseContent">The raw response body, if read.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    internal FeiertageApiHttpException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent,
        Uri? requestUri)
        : base(message, statusCode, responseContent, requestUri)
    {
    }

    /// <summary>
    /// Initializes a new instance for a transport-level failure (timeout, DNS, socket, etc.).
    /// Use when an <see cref="HttpRequestException"/> or <see cref="TaskCanceledException"/>
    /// (timeout) prevented the request from completing normally.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">A representative status code (e.g. <see cref="HttpStatusCode.RequestTimeout"/>
    /// for timeouts, or <see cref="HttpStatusCode.InternalServerError"/> for unclassified transport errors).</param>
    /// <param name="responseContent">The raw response body, if any was read before the failure.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    /// <param name="innerException">The underlying transport exception.</param>
    internal FeiertageApiHttpException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent,
        Uri? requestUri,
        HttpRequestException innerException)
        : base(message, statusCode, responseContent, requestUri, innerException)
    {
    }

    public override string ToString()
    {
        var baseString = base.ToString();

        if (StatusCode.HasValue)
        {
            baseString += $"{Environment.NewLine}HTTP Status Code: {(int)StatusCode.Value} ({StatusCode.Value})";
        }

        if (string.IsNullOrEmpty(ResponseContent))
            return baseString;

        var content = ResponseContent.Length > 500
            ? string.Concat(ResponseContent.AsSpan(0, 500), "...")
            : ResponseContent;
        baseString += $"{Environment.NewLine}Response Content: {content}";

        return baseString;
    }
}