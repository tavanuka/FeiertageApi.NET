using FeiertageApi.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace FeiertageApi.Extensions.AspNetCore.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFeiertageApi_ReturnsSameServiceCollection_ForFluentChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddFeiertageApi();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddFeiertageApi_RegistersIFeiertageApiClient()
    {
        var services = new ServiceCollection();
        services.AddFeiertageApi();
        using var provider = services.BuildServiceProvider();

        var client = provider.GetService<IFeiertageApiClient>();

        Assert.NotNull(client);
        Assert.IsType<FeiertageApiClient>(client);
    }

    [Fact]
    public void AddFeiertageApi_RegistersIHttpClientFactory()
    {
        var services = new ServiceCollection();
        services.AddFeiertageApi();
        using var provider = services.BuildServiceProvider();

        var factory = provider.GetService<IHttpClientFactory>();

        Assert.NotNull(factory);
    }

    [Fact]
    public void AddFeiertageApi_ConfiguresNamedHttpClientWithApiBaseUrl()
    {
        var services = new ServiceCollection();
        services.AddFeiertageApi();
        using var provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<IHttpClientFactory>();
        // AddHttpClient<TClient, TImpl>() registers under typeof(TClient).Name.
        var httpClient = factory.CreateClient(nameof(IFeiertageApiClient));

        Assert.Equal(new Uri(IFeiertageApiClient.FeiertageApiBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void AddFeiertageApi_ResolvesIndependentTransientInstancesPerScope()
    {
        var services = new ServiceCollection();
        services.AddFeiertageApi();
        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IFeiertageApiClient>();
        var second = provider.GetRequiredService<IFeiertageApiClient>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void AddFeiertageApi_CalledTwice_DoesNotThrowAndStillResolvesClient()
    {
        var services = new ServiceCollection();

        services.AddFeiertageApi();
        services.AddFeiertageApi();
        using var provider = services.BuildServiceProvider();

        var client = provider.GetService<IFeiertageApiClient>();
        Assert.NotNull(client);
    }
}