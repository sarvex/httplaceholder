﻿using System.Linq;
using System.Net;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubRedirectionIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularGet_TempRedirect()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}temp-redirect";

        // act / assert
        using var response = await Client.GetAsync(url);
        Assert.AreEqual(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        Assert.AreEqual("https://google.com/",
            response.Headers.Single(h => h.Key == HeaderKeys.Location).Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_PermanentRedirect()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}permanent-redirect";

        // act / assert
        using var response = await Client.GetAsync(url);
        Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.AreEqual("https://reddit.com/",
            response.Headers.Single(h => h.Key == HeaderKeys.Location).Value.Single());
    }
}
