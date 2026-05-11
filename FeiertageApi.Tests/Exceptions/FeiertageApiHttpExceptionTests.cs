using FeiertageApi.Exceptions;
using System.Net;

namespace FeiertageApi.Tests.Exceptions;

public class FeiertageApiHttpExceptionTests
{
    private static readonly Uri SampleUri = new("https://example.com/api?years=2024");

    [Fact]
    public void Constructor_WithoutInnerException_PopulatesAllProperties()
    {
        var ex = new FeiertageApiHttpException(
            "boom",
            HttpStatusCode.InternalServerError,
            "server exploded",
            SampleUri);

        Assert.Equal("boom", ex.Message);
        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.Equal("server exploded", ex.ResponseContent);
        Assert.Equal(SampleUri, ex.RequestUri);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Constructor_WithInnerException_PreservesInnerException()
    {
        var inner = new HttpRequestException("transport failed");

        var ex = new FeiertageApiHttpException(
            "wrapped",
            HttpStatusCode.RequestTimeout,
            null,
            SampleUri,
            inner);

        Assert.Same(inner, ex.InnerException);
        Assert.Equal(HttpStatusCode.RequestTimeout, ex.StatusCode);
        Assert.Null(ex.ResponseContent);
    }

    [Fact]
    public void ToString_IncludesStatusCodeAndResponseContent()
    {
        var ex = new FeiertageApiHttpException(
            "boom",
            HttpStatusCode.NotFound,
            "missing",
            SampleUri);

        var result = ex.ToString();

        Assert.Contains("HTTP Status Code: 404 (NotFound)", result);
        Assert.Contains("Response Content: missing", result);
    }

    [Fact]
    public void ToString_TruncatesResponseContent_WhenLongerThan500Chars()
    {
        var longBody = new string('x', 600);
        var ex = new FeiertageApiHttpException(
            "boom",
            HttpStatusCode.InternalServerError,
            longBody,
            SampleUri);

        var result = ex.ToString();

        Assert.Contains(new string('x', 500) + "...", result);
        Assert.DoesNotContain(new string('x', 600), result);
    }

    [Fact]
    public void ToString_OmitsResponseContentLine_WhenContentIsNull()
    {
        var ex = new FeiertageApiHttpException(
            "boom",
            HttpStatusCode.InternalServerError,
            responseContent: null,
            SampleUri);

        var result = ex.ToString();

        Assert.DoesNotContain("Response Content:", result);
    }
}