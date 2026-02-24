using FeiertageApi.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FeiertageApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Feiertage API client and its associated HTTP client to the service collection.
    /// Configures the HTTP client with the base address and applies a standard resilience handler.
    /// </summary>
    /// <param name="services">The service collection to which the Feiertage API client will be added.</param>
    /// <returns>The modified service collection with the Feiertage API client registered.</returns>
    public static IServiceCollection AddFeiertageApi(this IServiceCollection services)
    {
        services.AddHttpClient<IFeiertageApiClient, FeiertageApiClient>(static client => {
            client.BaseAddress = new Uri(IFeiertageApiClient.FeiertageApiBaseUrl);
        })
        .AddStandardResilienceHandler();
        
        return services;
    }
}