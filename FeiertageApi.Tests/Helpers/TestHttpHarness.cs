using FeiertageApi.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace FeiertageApi.Tests.Helpers;

/// <summary>
/// Owns the handler, HttpClient and <see cref="FeiertageApiClient"/> for a single test.
/// Disposing the harness disposes the HttpClient (which cascades to the handler) and the client.
/// </summary>
internal sealed class TestHttpHarness : IDisposable
{
    private readonly HttpClient _httpClient;

    public StubHttpMessageHandler Handler { get; }
    public FeiertageApiClient Client { get; }

    public TestHttpHarness(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        Handler = new StubHttpMessageHandler(respond);
        _httpClient = new HttpClient(Handler)
        {
            BaseAddress = new Uri(IFeiertageApiClient.FeiertageApiBaseUrl)
        };
        Client = new FeiertageApiClient(_httpClient, NullLogger<FeiertageApiClient>.Instance);
    }

    public static TestHttpHarness Returning(string jsonContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(_ => new HttpResponseMessage(statusCode) { Content = new StringContent(jsonContent) });

    public void Dispose()
    {
        Client.Dispose();
        _httpClient.Dispose();
    }
}
