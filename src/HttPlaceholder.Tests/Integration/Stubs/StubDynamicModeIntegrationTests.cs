﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Web.Shared.Dto.v1.Scenarios;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubDynamicModeIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_Query()
    {
        // Arrange
        const string query1Val = "John";
        const string query2Val = "=";
        var expectedResult = $"The value is {query1Val} {WebUtility.UrlEncode(query2Val)}";
        var url = $"{TestServer.BaseAddress}dynamic-query.txt?queryString1={query1Val}&queryString2={query2Val}";

        // Act / Assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_Uuid()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-uuid.txt";

        // Act / Assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(Guid.TryParse(content.Replace("The value is ", string.Empty), out _));
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_RequestHeaders()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var expectedResult = $"API key: {apiKey}";
        var url = $"{TestServer.BaseAddress}dynamic-request-header.txt";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Api-Key", apiKey);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual("localhost", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Dynamic_FormPost()
    {
        // Arrange
        const string expectedResult = "Posted: Value 1!";
        var url = $"{TestServer.BaseAddress}dynamic-form-post.txt";

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"formval1", "Value 1!"}, {"formval2", "Value 2!"}
            })
        };

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual("Value 2!", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Dynamic_RequestBody()
    {
        // Arrange
        const string expectedResult = "Posted: Test123";
        var url = $"{TestServer.BaseAddress}dynamic-request-body.txt";
        const string body = "Test123";

        var request = new HttpRequestMessage(HttpMethod.Post, url) {Content = new StringContent(body)};

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual("Test123", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Dynamic_RequestBodyRegex()
    {
        // Arrange
        const string expectedResult = "Posted: value2";
        var url = $"{TestServer.BaseAddress}dynamic-request-body-regex.txt";
        const string body = @"key1=value1
key2=value2
key3=value3";

        var request = new HttpRequestMessage(HttpMethod.Post, url) {Content = new StringContent(body)};

        // Act / Assert
        using var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual("value3", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_DisplayUrl()
    {
        // Arrange
        const string query = "?var1=value1&var2=value2";
        var url = $"{TestServer.BaseAddress}dynamic-display-url.txt{query}";
        var expectedResult = $"URL: {url}";

        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("localhost");

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(url, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_DisplayUrl_Regex()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-display-url-regex/users/123/orders?key=val";
        const string expectedResult = "User ID: 123";

        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("localhost");

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual("123", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_RootUrl()
    {
        // Arrange
        const string query = "?var1=value1&var2=value2";
        var url = $"{TestServer.BaseAddress}dynamic-root-url.txt{query}";
        var baseUrl = TestServer.BaseAddress.OriginalString.TrimEnd('/');
        var expectedResult = $"URL: {baseUrl}";

        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("localhost");

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(baseUrl, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_ClientIp()
    {
        // Arrange
        const string ip = "11.22.33.44";
        var url = $"{TestServer.BaseAddress}dynamic-client-ip.txt";
        const string expectedResult = $"IP: {ip}";

        ClientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(ip, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_LocalNow()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-local-now.txt";
        const string expectedDateTime = "2019-08-21 20:41:51";
        const string expectedResult = $"Local now: {expectedDateTime}";

        var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Local);
        DateTimeMock
            .Setup(m => m.Now)
            .Returns(now);

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(expectedDateTime, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_UtcNow()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-utc-now.txt";
        const string expectedDateTime = "2019-08-21 20:41:51";
        const string expectedResult = $"UTC now: {expectedDateTime}";

        var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Utc);
        DateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(now);

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(expectedDateTime, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Dynamic_JsonPath()
    {
        // Arrange
        const string expectedResult = "JSONPath result: Value2";
        var url = $"{TestServer.BaseAddress}dynamic-mode-jsonpath.txt";
        const string body = @"{
    ""values"": [
        {
            ""title"": ""Value1""
        },
        {
            ""title"": ""Value2""
        }
    ]
}";

        var request = new HttpRequestMessage(HttpMethod.Post, url) {Content = new StringContent(body)};

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("Value1", response.Headers.Single(h => h.Key == "X-Value").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_ScenarioState()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-mode-scenario-state.txt";
        await SetScenario("dynamic-mode-scenario-state", new ScenarioStateInputDto {State = "cool_state_1"});
        await SetScenario("scenario123", new ScenarioStateInputDto {State = "cool_state_2"});
        const string expectedResult = "cool_state_1 cool_state_2";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(expectedResult, response.Headers.Single(h => h.Key == "X-Value").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_ScenarioHitCount()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-mode-scenario-hitcount.txt";
        await SetScenario("dynamic-mode-scenario-hitcount", new ScenarioStateInputDto {HitCount = 3});
        await SetScenario("scenario123", new ScenarioStateInputDto {HitCount = 123});
        const string expectedResult = "4 123";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act / Assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expectedResult, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());

        Assert.AreEqual(expectedResult, response.Headers.Single(h => h.Key == "X-Value").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Dynamic_FakeData()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}dynamic-mode-fake-data.txt";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("first_name:"));
        Assert.IsFalse(content.Contains("(("));
    }
}
