﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class FullPathConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void FullPathConditionChecker_Validate_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FullPathConditionChecker>();

        var conditions = new StubConditionsModel {Url = new StubUrlConditionModel {FullPath = null}};

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void FullPathConditionChecker_Validate_StubsFound_WrongPath_ShouldReturnInvalid()
    {
        // Arrange
        const string path = "/login?success=true";

        var checker = _mocker.CreateInstance<FullPathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Url = new StubUrlConditionModel {FullPath = "/login?success=false"}};

        httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.FullPath))
            .Returns(false);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void FullPathConditionChecker_Validate_StubsFound_HappyFlow_CompleteUrl()
    {
        // Arrange
        const string path = "/login?success=true";

        var checker = _mocker.CreateInstance<FullPathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Url = new StubUrlConditionModel {FullPath = "/login?success=true"}};

        httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.FullPath))
            .Returns(true);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
