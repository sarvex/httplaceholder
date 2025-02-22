﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to communicate with the backing stub sources.
/// </summary>
public interface IStubContext
{
    /// <summary>
    ///     Returns a list of <see cref="FullStubModel" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="FullStubModel" />.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a list of <see cref="FullStubModel" /> from read-only sources.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="FullStubModel" /> from read-only sources.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsFromReadOnlySourcesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a list of <see cref="FullStubModel" /> by tenant.
    /// </summary>
    /// <param name="tenant">The tenant name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="FullStubModel" /> by tenant.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant, CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a list of <see cref="FullStubOverviewModel" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="FullStubOverviewModel" />.</returns>
    Task<IEnumerable<FullStubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Adds a stub.
    /// </summary>
    /// <param name="stub">The <see cref="StubModel" /> to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added <see cref="FullStubModel" />.</returns>
    Task<FullStubModel> AddStubAsync(StubModel stub, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a stub by stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the stub was deleted, false otherwise.</returns>
    Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes all stubs by tenant.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="tenant">The tenant name.</param>
    Task DeleteAllStubsAsync(string tenant, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes all stubs.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllStubsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Updates all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant name.</param>
    /// <param name="stubs">The stubs that should be updated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a stub by stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="FullStubModel" />.</returns>
    Task<FullStubModel> GetStubAsync(string stubId, CancellationToken cancellationToken);

    /// <summary>
    ///     Adds a <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="requestResult">The request to add.</param>
    /// <param name="responseModel">The response that belongs to the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a list of <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="RequestResultModel" />.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a list of <see cref="RequestOverviewModel" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="RequestOverviewModel" />.</returns>
    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a list of <see cref="RequestResultModel" /> by specific stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="RequestResultModel" />.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a specific <see cref="RequestResultModel" /> based on correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="RequestResultModel" />.</returns>
    Task<RequestResultModel> GetRequestResultAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a specific <see cref="ResponseModel" /> based on request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ResponseModel" />.</returns>
    Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes all requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the request was deleted, false otherwise.</returns>
    Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Clean all old requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CleanOldRequestResultsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a list with all tenant names.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list with all tenant names.</returns>
    Task<IEnumerable<string>> GetTenantNamesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Prepares all stub sources.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PrepareAsync(CancellationToken cancellationToken);
}
