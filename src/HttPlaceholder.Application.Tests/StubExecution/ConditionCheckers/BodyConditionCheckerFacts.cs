﻿using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class BodyConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void BodyConditionChecker_Validate_StubsFound_ButNoBodyConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var conditions = new StubConditionsModel {Body = null};

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void BodyConditionChecker_Validate_StubsFound_AllBodyConditionsIncorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel {Body = new[] {@"\bthat\b", @"\btree\b"}};

        httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void BodyConditionChecker_Validate_StubsFound_OnlyOneBodyConditionCorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Body = new[] {@"\bthis\b", @"\btree\b"}};

        httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(0)))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(1)))
            .Returns(false);

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void BodyConditionChecker_Validate_StubsFound_HappyFlow_FullText()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Body = new[] {"this is a test"}};

        httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(0)))
            .Returns(true);

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
