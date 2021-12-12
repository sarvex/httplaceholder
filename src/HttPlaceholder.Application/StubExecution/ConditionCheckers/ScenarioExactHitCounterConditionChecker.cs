﻿using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

public class ScenarioExactHitCounterConditionChecker : IConditionChecker
{
    private readonly IScenarioService _scenarioService;

    public ScenarioExactHitCounterConditionChecker(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var exactHits = stub.Conditions?.Scenario?.ExactHits;
        if (exactHits == null)
        {
            return result;
        }

        var scenario = stub.Scenario;
        var rawHitCount = _scenarioService.GetHitCount(scenario);
        var actualHitCount = rawHitCount + 1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
        if (actualHitCount == null)
        {
            result.Log = "No hit count could be found.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount != exactHits)
        {
            result.Log =
                $"Scenario '{scenario}' should have exactly '{exactHits}' hits, but '{actualHitCount}' hits were counted.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else
        {
            result.ConditionValidation = ConditionValidationType.Valid;
        }

        return result;
    }

    public int Priority => 8;
}