using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// "Request to stub conditions handler" that is used to create a request path (no query parameters) expression.
/// </summary>
internal class PathHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        var uri = new Uri(request.Url);
        conditions.Url.Path = new StubConditionStringCheckingModel {StringEquals = uri.LocalPath};
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
