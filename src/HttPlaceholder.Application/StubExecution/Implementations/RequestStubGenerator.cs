using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class RequestStubGenerator : IRequestStubGenerator, ISingletonService
{
    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly ILogger<RequestStubGenerator> _logger;
    private readonly IMapper _mapper;
    private readonly IStubContext _stubContext;

    public RequestStubGenerator(
        IStubContext stubContext,
        ILogger<RequestStubGenerator> logger,
        IMapper mapper,
        IHttpRequestToConditionsService httpRequestToConditionsService)
    {
        _stubContext = stubContext;
        _logger = logger;
        _mapper = mapper;
        _httpRequestToConditionsService = httpRequestToConditionsService;
    }

    /// <inheritdoc />
    public async Task<FullStubModel> GenerateStubBasedOnRequestAsync(
        string requestCorrelationId,
        bool doNotCreateStub,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Creating stub based on request with corr.ID '{requestCorrelationId}'.");
        var requestResult = await _stubContext.GetRequestResultAsync(requestCorrelationId, cancellationToken)
            .IfNull(() => throw new NotFoundException(nameof(RequestResultModel), requestCorrelationId));
        var request = _mapper.Map<HttpRequestModel>(requestResult.RequestParameters);
        var stub = new StubModel
        {
            Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken),
            Response = {Text = "OK!"}
        };

        // Generate an ID based on the created stub.
        stub.EnsureStubId();

        FullStubModel result;
        if (doNotCreateStub)
        {
            result = new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }
        else
        {
            await _stubContext.DeleteStubAsync(stub.Id, cancellationToken);
            result = await _stubContext.AddStubAsync(stub, cancellationToken);
        }

        _logger.LogDebug($"Stub with ID '{stub.Id}' generated!");

        return result;
    }
}
