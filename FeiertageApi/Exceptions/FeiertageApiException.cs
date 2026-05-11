using System;
using System.Net;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Base exception type for all Feiertage API related errors. Throw one of the concrete
/// subclasses (<see cref="FeiertageApiHttpException"/>, <see cref="FeiertageApiResponseException"/>);
/// callers can catch this base type to handle any failure surfaced by the client.
/// </summary>
public abstract class FeiertageApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code of the response, if a response was received.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the raw response body, if one was read.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the URI that was requested, if known.
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    /// Initializes a new instance with a message only.
    /// Intended for failures that have no HTTP context — for example, an
    /// <see cref="ObjectDisposedException"/>-style precondition or argument validation
    /// inside the client before a request was issued.
    /// </summary>
    protected FeiertageApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance wrapping an underlying exception, with no HTTP context.
    /// Intended for failures detected before a request was issued where the cause is itself
    /// an exception (e.g. invalid arguments).
    /// </summary>
    protected FeiertageApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance with HTTP response context.
    /// Intended for failures that are themselves the API's response (e.g. a non-success
    /// status code, an unparseable body, or an <c>"error"</c> status field) rather than a
    /// surfacing of a lower-level exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code of the response, if available.</param>
    /// <param name="responseContent">The raw response body, if available.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    protected FeiertageApiException(
        string message,
        HttpStatusCode? statusCode,
        string? responseContent,
        Uri? requestUri)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance with HTTP response context, wrapping the underlying
    /// exception that triggered the failure.
    /// Intended for transport-level errors (e.g. <see cref="System.Net.Http.HttpRequestException"/>)
    /// or body-parsing errors (e.g. <see cref="System.Text.Json.JsonException"/>).
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code of the response, if available.</param>
    /// <param name="responseContent">The raw response body, if available.</param>
    /// <param name="requestUri">The URI that was requested.</param>
    /// <param name="innerException">The underlying exception that caused this failure.</param>
    protected FeiertageApiException(
        string message,
        HttpStatusCode? statusCode,
        string? responseContent,
        Uri? requestUri,
        Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        RequestUri = requestUri;
    }
}