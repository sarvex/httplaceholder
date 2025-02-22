using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class FormHandlerFacts
{
    private readonly FormHandler _handler = new();

    [TestMethod]
    public async Task FormHandler_HandleStubGenerationAsync_NoContentTypeSet_ShouldReturnFalse()
    {
        // Arrange
        var request = new HttpRequestModel {Headers = new Dictionary<string, string>()};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Form);
    }

    [TestMethod]
    public async Task FormHandler_HandleStubGenerationAsync_NoFormContentTypeSet_ShouldReturnFalse()
    {
        // Arrange
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string> {{HeaderKeys.ContentType, MimeTypes.JsonMime}}
        };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Form);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public async Task FormHandler_HandleStubGenerationAsync_NoRequestBodySet_ShouldReturnFalse(string body)
    {
        // Arrange
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string> {{HeaderKeys.ContentType, MimeTypes.UrlEncodedFormMime}},
            Body = body
        };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Form);
    }

    [TestMethod]
    public async Task FormHandler_FormReaderReturnsNoValues_ShouldReturnFalse()
    {
        // Arrange
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string> {{HeaderKeys.ContentType, MimeTypes.UrlEncodedFormMime}},
            Body = "invalid form body"
        };
        var conditions = new StubConditionsModel {Body = new[] {"body1", "body2"}};

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(2, conditions.Body.Count());
    }

    [DataTestMethod]
    [DataRow(MimeTypes.UrlEncodedFormMime)]
    [DataRow($"{MimeTypes.UrlEncodedFormMime}; charset=UTF-8")]
    [DataRow(MimeTypes.MultipartFormDataMime)]
    [DataRow($"{MimeTypes.MultipartFormDataMime}; charset=UTF-8")]
    public async Task FormHandler_HandleStubGenerationAsync_HappyFlow(string contentType)
    {
        // Arrange
        const string form = "form1=val1&form2=val2";
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string> {{HeaderKeys.ContentType, contentType}}, Body = form
        };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);

        var formDict = conditions.Form.ToDictionary(f => f.Key, f => f.Value);
        Assert.AreEqual("val1", ((StubConditionStringCheckingModel)formDict["form1"]).StringEquals);
        Assert.AreEqual("val2", ((StubConditionStringCheckingModel)formDict["form2"]).StringEquals);
        Assert.IsFalse(conditions.Body.Any());
    }
}
