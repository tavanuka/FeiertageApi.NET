using System;
using System.Net;
using System.Net.Http;

namespace FeiertageApi.Exceptions;

/// <summary>
/// Exception thrown when an HTTP error occurs while communicating with the Feiertage API.
/// </summary>
public sealed class FeiertageApiHttpException : FeiertageApiException
{
    public FeiertageApiHttpException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent = null,
        Uri? requestUri = null,
        HttpRequestException? innerException = null)
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
