using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Dto.v1.Scenarios;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class ScenarioNotify : IScenarioNotify
{
    private readonly IHubContext<ScenarioHub> _hubContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ScenarioNotify> _logger;

    /// <summary>
    ///     Constructs a <see cref="ScenarioNotify" /> instance.
    /// </summary>
    public ScenarioNotify(IHubContext<ScenarioHub> hubContext, IMapper mapper, ILogger<ScenarioNotify> logger)
    {
        _hubContext = hubContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ScenarioSetAsync(ScenarioStateModel scenario, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"ScenarioSet triggered: {scenario}");
        var input = _mapper.Map<ScenarioStateDto>(scenario);
        await _hubContext.Clients.All.SendAsync("ScenarioSet", input, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ScenarioDeletedAsync(string scenarioName, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"ScenarioDeleted triggered: {scenarioName}");
        await _hubContext.Clients.All.SendAsync("ScenarioDeleted", scenarioName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AllScenariosDeletedAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("AllScenariosDeleted triggered.");
        await _hubContext.Clients.All.SendAsync("AllScenariosDeleted", cancellationToken);
    }
}
