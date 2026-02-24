using FeiertageApi.Exceptions;
using FeiertageApi.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FeiertageApi.Clients;

public interface IFeiertageApiClient
{
    /// <summary>
    /// Represents the base URL of the Feiertage API.
    /// This property provides the endpoint that the <see cref="IFeiertageApiClient"/> implementation uses
    /// to communicate with the Feiertage API for retrieving public holidays data.
    /// </summary>
    public static string FeiertageApiBaseUrl => "https://get.feiertage-api.de/";

    Task<HolidayResponse> GetPublicHolidays(bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default);
    Task<HolidayResponse> GetPublicHolidays(int year, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default);
    Task<HolidayResponse> GetPublicHolidays(int year, string state, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default);
    Task<HolidayResponse> GetPublicHolidays(int year, IEnumerable<string> states, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default);
    Task<HolidayResponse> GetPublicHolidays(IEnumerable<int> years, IEnumerable<string> states, bool allStates = false, bool? catholic = null, bool? augsburg = null, CancellationToken cancellationToken = default);

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
    IAsyncEnumerable<Holiday> StreamPublicHolidays(
        HolidayRequest request,
        CancellationToken cancellationToken = default);
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
    IAsyncEnumerable<Holiday> StreamPublicHolidays(
        int year,
        string state,
        bool allStates = false,
        bool? catholic = null,
        bool? augsburg = null,
        CancellationToken cancellationToken = default);
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
    IAsyncEnumerable<Holiday> StreamPublicHolidays(
        IEnumerable<int> years,
        IEnumerable<string> states,
        bool allStates = false,
        bool? catholic = null,
        bool? augsburg = null,
        CancellationToken cancellationToken = default);
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
    Task<HolidayResponse> GetPublicHolidays(HolidayRequest request, CancellationToken cancellationToken = default);
}