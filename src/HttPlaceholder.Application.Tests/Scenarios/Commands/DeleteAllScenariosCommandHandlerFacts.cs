﻿using HttPlaceholder.Application.Scenarios.Commands.DeleteAllScenarios;
using HttPlaceholder.Application.StubExecution;

namespace HttPlaceholder.Application.Tests.Scenarios.Commands;

[TestClass]
public class DeleteAllScenariosCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<DeleteAllScenariosCommandHandler>();

        // Act
        await handler.Handle(new DeleteAllScenariosCommand(), CancellationToken.None);

        // Assert
        scenarioServiceMock.Verify(m => m.DeleteAllScenariosAsync(It.IsAny<CancellationToken>()));
    }
}
