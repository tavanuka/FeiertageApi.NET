namespace FeiertageApi.Tests.Helpers;

/// <summary>
/// Hand-rolled HttpMessageHandler stub for unit tests. Captures the last request and
/// returns a response produced by the supplied factory.
/// </summary>
internal sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(respond(request));
    }
}