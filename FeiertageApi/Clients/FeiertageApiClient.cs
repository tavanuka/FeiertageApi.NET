using FeiertageApi.Exceptions;
using FeiertageApi.Models;
using FeiertageApi.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FeiertageApi.Clients;

internal sealed class FeiertageApiClient : IFeiertageApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FeiertageApiClient> _logger;

    public FeiertageApiClient(HttpClient httpClient, ILogger<FeiertageApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger ?? NullLogger<FeiertageApiClient>.Instance;
    }

    public async Task<HolidayResponse> GetPublicHolidays(bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default)
        => await GetPublicHolidays(new HolidayRequest([], [], allStates, catholic, augsburg), cancellationToken);

    public async Task<HolidayResponse> GetPublicHolidays(int year, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default)
        => await GetPublicHolidays(new HolidayRequest([year], [], allStates, catholic, augsburg), cancellationToken);

    public async Task<HolidayResponse> GetPublicHolidays(int year, string state, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default)
        => await GetPublicHolidays(new HolidayRequest([year], [state], allStates, catholic, augsburg), cancellationToken);

    public async Task<HolidayResponse> GetPublicHolidays(int year, IEnumerable<string> states, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default)
        => await GetPublicHolidays(new HolidayRequest([year], states, allStates, catholic, augsburg), cancellationToken);

    public async Task<HolidayResponse> GetPublicHolidays(IEnumerable<int> years, IEnumerable<string> states, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default)
        => await GetPublicHolidays(new HolidayRequest(years, states, allStates, catholic, augsburg), cancellationToken);

    /// <summary>
    /// Streams public holidays based on the specified criteria, yielding one holiday at a time.
    /// This method is useful for processing large holiday datasets without loading all data into memory.
    /// </summary>
    /// <param name="request">
    /// An instance of <see cref="HolidayRequest"/> that contains the filtering criteria,
    /// including years, states, and optional flags such as AllStates, Catholic, and Augsburg.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An asynchronous enumerable of <see cref="Holiday"/> objects.
    /// </returns>
    /// <exception cref="FeiertageApiHttpException">
    /// Thrown when an HTTP error occurs while communicating with the API.
    /// </exception>
    /// <exception cref="FeiertageApiResponseException">
    /// Thrown when the API returns an error status or the response cannot be parsed.
    /// </exception>
    public async IAsyncEnumerable<Holiday> StreamPublicHolidays(
        HolidayRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Streaming holidays from Feiertage API");

        var response = await GetPublicHolidays(request, cancellationToken);

        foreach (var holiday in response.Holidays)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return holiday;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Finished streaming {HolidayCount} holidays", response.Holidays.Count);
    }

    /// <summary>
    /// Streams public holidays for a specific year and state.
    /// </summary>
    /// <param name="year">The year for which holidays are requested.</param>
    /// <param name="state">The state code (e.g., "by" for Bavaria).</param>
    /// <param name="allStates">Include only holidays valid in all states.</param>
    /// <param name="catholic">Filter by Catholic holidays.</param>
    /// <param name="augsburg">Filter by Augsburg-specific holidays.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An asynchronous enumerable of holidays.</returns>
    public IAsyncEnumerable<Holiday> StreamPublicHolidays(
        int year,
        string state,
        bool allStates = false,
        bool? catholic = null,
        bool? augsburg = null,
        CancellationToken cancellationToken = default)
        => StreamPublicHolidays(new HolidayRequest([year], [state], allStates, catholic, augsburg), cancellationToken);

    /// <summary>
    /// Streams public holidays for multiple years and states.
    /// </summary>
    /// <param name="years">Collection of years for which holidays are requested.</param>
    /// <param name="states">Collection of state codes.</param>
    /// <param name="allStates">Include only holidays valid in all states.</param>
    /// <param name="catholic">Filter by Catholic holidays.</param>
    /// <param name="augsburg">Filter by Augsburg-specific holidays.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An asynchronous enumerable of holidays.</returns>
    public IAsyncEnumerable<Holiday> StreamPublicHolidays(
        IEnumerable<int> years,
        IEnumerable<string> states,
        bool allStates = false,
        bool? catholic = null,
        bool? augsburg = null,
        CancellationToken cancellationToken = default)
        => StreamPublicHolidays(new HolidayRequest(years, states, allStates, catholic, augsburg), cancellationToken);

    /// <summary>
    /// Retrieves public holidays based on the specified criteria.
    /// </summary>
    /// <param name="request">
    /// An instance of <see cref="HolidayRequest"/> that contains the filtering criteria,
    /// including years, states, and optional flags such as AllStates, Catholic, and Augsburg.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="HolidayResponse"/> object containing the status, a list of holidays,
    /// and an optional error message if the request was unsuccessful.
    /// </returns>
    /// <exception cref="FeiertageApiHttpException">
    /// Thrown when an HTTP error occurs while communicating with the API.
    /// </exception>
    /// <exception cref="FeiertageApiResponseException">
    /// Thrown when the API returns an error status or the response cannot be parsed.
    /// </exception>
    public async Task<HolidayResponse> GetPublicHolidays(HolidayRequest request, CancellationToken cancellationToken = default)
    {
        var queryDict = new Dictionary<string, string?>();
        if (request.AllStates)
            queryDict.Add("all_states", "true");

        if (request.States.Any())
            queryDict.Add("states", string.Join(",", request.States));

        if (request.Years.Any())
            queryDict.Add("years", string.Join(",", request.Years));

        if (request.Catholic.HasValue)
            queryDict.Add("catholic", request.Catholic.Value.ToString().ToLowerInvariant());

        if (request.Augsburg.HasValue)
            queryDict.Add("augsburg", request.Augsburg.Value.ToString().ToLowerInvariant());

        var query = QueryHelpers.AddQueryString(string.Empty, queryDict);
        var requestUri = new Uri(_httpClient.BaseAddress!, query);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Requesting holidays from Feiertage API: {RequestUri}", requestUri);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(query, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while calling Feiertage API: {RequestUri}", requestUri);
            throw new FeiertageApiHttpException(
                "Failed to communicate with the Feiertage API due to an HTTP error.",
                ex.StatusCode ?? System.Net.HttpStatusCode.InternalServerError,
                null,
                requestUri,
                ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request to Feiertage API timed out: {RequestUri}", requestUri);
            throw new FeiertageApiHttpException(
                "The request to the Feiertage API timed out.",
                System.Net.HttpStatusCode.RequestTimeout,
                null,
                requestUri,
                new HttpRequestException("Request timeout", ex));
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Feiertage API returned non-success status code {StatusCode}: {ResponseContent}",
                response.StatusCode, responseContent);
            throw new FeiertageApiHttpException(
                $"The Feiertage API returned an error status code: {response.StatusCode}.",
                response.StatusCode,
                responseContent,
                requestUri);
        }

        HolidayResponse? holidayResponse;
        try
        {
            holidayResponse = System.Text.Json.JsonSerializer.Deserialize<HolidayResponse>(responseContent);
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from Feiertage API: {ResponseContent}", responseContent);
            throw new FeiertageApiResponseException(
                "The response from the Feiertage API could not be parsed.",
                statusCode: response.StatusCode,
                responseContent: responseContent,
                requestUri: requestUri);
        }

        if (holidayResponse == null)
        {
            _logger.LogError("Deserialized response from Feiertage API was null: {ResponseContent}", responseContent);
            throw new FeiertageApiResponseException(
                "The response from the Feiertage API was empty or invalid.",
                statusCode: response.StatusCode,
                responseContent: responseContent,
                requestUri: requestUri);
        }

        // Validate response status
        if (holidayResponse.Status.Equals("error", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Feiertage API returned error status: {ErrorMessage}", holidayResponse.ErrorMessage);
            throw new FeiertageApiResponseException(
                "The Feiertage API returned an error response.",
                apiStatus: holidayResponse.Status,
                errorDescription: holidayResponse.ErrorMessage,
                statusCode: response.StatusCode,
                responseContent: responseContent,
                requestUri: requestUri);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Successfully retrieved {HolidayCount} holidays from Feiertage API", holidayResponse.Holidays.Count);

        return holidayResponse;
    }
}