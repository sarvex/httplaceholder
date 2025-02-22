﻿using System.Linq;
using System.Net;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiRequestIntegrationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Request_GetAll()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestResultDto[]>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(correlation, result.First().CorrelationId);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetSingle_RequestNotFound_ShouldReturn404()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}1");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetSingle_RequestFound_ShouldReturnRequest()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestResultDto>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(correlation, result.CorrelationId);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetResponse_RequestNotFound_ShouldReturn404()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        var request = new RequestResultModel {CorrelationId = correlation};
        var responseModel = new ResponseModel();
        StubSource.RequestResultModels.Add(request);
        StubSource.StubResponses.Add(responseModel);
        StubSource.RequestResponseMap.Add(request, responseModel);

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}1/response");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetResponse_RequestFound_ShouldReturnResponse()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        var request = new RequestResultModel {CorrelationId = correlation};
        var responseModel = new ResponseModel {Body = new byte[] {1, 2, 3}, StatusCode = 200, BodyIsBinary = true};
        StubSource.RequestResultModels.Add(request);
        StubSource.StubResponses.Add(responseModel);
        StubSource.RequestResponseMap.Add(request, responseModel);

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}/response");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseDto>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Body.SequenceEqual(responseModel.Body));
        Assert.AreEqual(responseModel.StatusCode, result.StatusCode);
        Assert.AreEqual(responseModel.BodyIsBinary, result.BodyIsBinary);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetOverview()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/overview");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestOverviewDto[]>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(correlation, result.First().CorrelationId);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_GetByStubId()
    {
        // Arrange
        StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub2"});
        StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub1"});

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/stubs/stub1/requests");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestResultDto[]>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("stub1", result.First().ExecutingStubId);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_DeleteAllRequests()
    {
        // Perform a few requests.
        StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub2"});
        StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub1"});

        // Act
        using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/requests");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual(0, StubSource.RequestResultModels.Count);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_DeleteRequest()
    {
        // Perform a few requests.
        var request1 =
            new RequestResultModel {ExecutingStubId = "stub1", CorrelationId = Guid.NewGuid().ToString()};
        var request2 =
            new RequestResultModel {ExecutingStubId = "stub2", CorrelationId = Guid.NewGuid().ToString()};
        StubSource.RequestResultModels.Add(request1);
        StubSource.RequestResultModels.Add(request2);

        // Act
        using var response =
            await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/requests/{request2.CorrelationId}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual(1, StubSource.RequestResultModels.Count);
        Assert.IsTrue(StubSource.RequestResultModels.Any(r => r.CorrelationId == request1.CorrelationId));
    }
}
