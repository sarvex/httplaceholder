﻿using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Web.Shared.Authorization.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Web.Shared.Tests.Authorization;

[TestClass]
public class LoginServiceFacts
{
    private readonly Dictionary<string, string> _cookies = new();
    private readonly MockHttpContext _mockHttpContext = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();
    private LoginService _service;

    [TestInitialize]
    public void Initialize()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockHttpContext);

        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.Cookies)
            .Returns(() => new RequestCookieCollection(_cookies));

        _service = new LoginService(accessorMock.Object, _options);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_NoUsernameAndPasswordSet_ShouldReturnTrue()
    {
        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_NoCookieSet_ShouldReturnFalse()
    {
        // Arrange
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashIncorrect_ShouldReturnFalse()
    {
        // Arrange
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";
        _cookies.Add("HttPlaceholderLoggedin", "INCORRECT");

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashCorrect_ShouldReturnTrue()
    {
        // Arrange
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";
        _cookies.Add("HttPlaceholderLoggedin",
            "qkUYd4wTaLeznD/nN1v9ei9/5XUekWt1hyOctq3bQZ9DMhSk7FJz+l1ILk++kyYlu+VguxVcuEC9R4Ryk763GA==");

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }
}
