using FeiertageApi.Exceptions;
using System.Net;

namespace FeiertageApi.Tests.Exceptions;

public class FeiertageApiResponseExceptionTests
{
    private static readonly Uri SampleUri = new("https://example.com/api?years=2024");

    [Fact]
    public void Constructor_BasicCtor_LeavesApiFieldsNull()
    {
        var ex = new FeiertageApiResponseException(
            "could not parse",
            HttpStatusCode.OK,
            "garbage body",
            SampleUri);

        Assert.Null(ex.ApiStatus);
        Assert.Null(ex.ErrorDescription);
        Assert.Equal(HttpStatusCode.OK, ex.StatusCode);
        Assert.Equal("garbage body", ex.ResponseContent);
        Assert.Equal(SampleUri, ex.RequestUri);
    }

    [Fact]
    public void Constructor_ApiErrorCtor_PopulatesApiStatusAndErrorDescription()
    {
        var ex = new FeiertageApiResponseException(
            "API said error",
            apiStatus: "error",
            errorDescription: "Rate limit exceeded",
            HttpStatusCode.OK,
            """{"status":"error"}""",
            SampleUri);

        Assert.Equal("error", ex.ApiStatus);
        Assert.Equal("Rate limit exceeded", ex.ErrorDescription);
        Assert.Equal(HttpStatusCode.OK, ex.StatusCode);
    }

    [Fact]
    public void ToString_IncludesErrorDescriptionAndRequestUri_WhenPresent()
    {
        var ex = new FeiertageApiResponseException(
            "API said error",
            apiStatus: "error",
            errorDescription: "Rate limit exceeded",
            HttpStatusCode.OK,
            responseContent: null,
            SampleUri);

        var result = ex.ToString();

        Assert.Contains("API Error: Rate limit exceeded", result);
        Assert.Contains($"Request URI: {SampleUri}", result);
    }

    [Fact]
    public void ToString_OmitsErrorDescriptionLine_WhenNotProvided()
    {
        var ex = new FeiertageApiResponseException(
            "could not parse",
            HttpStatusCode.OK,
            "garbage body",
            SampleUri);

        var result = ex.ToString();

        Assert.DoesNotContain("API Error:", result);
        // RequestUri is still present via the basic ctor's path.
        Assert.Contains($"Request URI: {SampleUri}", result);
    }
}