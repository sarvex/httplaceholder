﻿using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain.Entities;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class ScenarioHitCountVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Parse_NoMatches_ShouldReturnInputAsIs()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ScenarioHitCountVariableParsingHandler>();
        const string input = "the input";

        // Act
        var result = await handler.ParseAsync(input, Array.Empty<Match>(), new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task Parse_Matches_ShouldParseHitCount()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ScenarioHitCountVariableParsingHandler>();
        var mockScenarioStateStore = _mocker.GetMock<IScenarioStateStore>();
        const string input =
            "((scenario_hitcount)) ((scenario_hitcount:scenario_exists)) ((scenario_hitcount:scenario_doesnt_exist))";
        const string expectedResult = "3 1337 ";

        var stubModel = new StubModel {Scenario = "stub-scenario"};

        mockScenarioStateStore
            .Setup(m => m.GetScenario(stubModel.Scenario))
            .Returns(new ScenarioStateModel {HitCount = 3});
        mockScenarioStateStore
            .Setup(m => m.GetScenario("scenario_exists"))
            .Returns(new ScenarioStateModel {HitCount = 1337});
        mockScenarioStateStore
            .Setup(m => m.GetScenario("scenario_doesnt_exist"))
            .Returns((ScenarioStateModel)null);

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, stubModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
