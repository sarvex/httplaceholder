using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Authorization.Implementations;

internal class LoginService : ILoginService, ITransientService
{
    private const string LoginCookieKey = "HttPlaceholderLoggedin";
    private const string Salt = "83b2737f-7d85-4a0a-8113-b98ed4a255a1";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly ILogger<LoginService> _logger;

    public LoginService(
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<SettingsModel> options, ILogger<LoginService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool CheckLoginCookie()
    {
        var settings = _options.CurrentValue;
        var username = settings.Authentication?.ApiUsername ?? string.Empty;
        var password = settings.Authentication?.ApiPassword ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            _logger.LogDebug("No username and password set, so returning true.");
            return true;
        }

        var expectedHash = CreateHash(username, password);
        var cookie =
            _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == LoginCookieKey);
        var result = cookie.Value == expectedHash;
        _logger.LogDebug("Result of login: {}.", result);
        return result;
    }

    /// <inheritdoc />
    public void SetLoginCookie(string username, string password) =>
        _httpContextAccessor.HttpContext.Response.Cookies.Append(
            LoginCookieKey,
            CreateHash(username, password),
            new CookieOptions {HttpOnly = false, SameSite = SameSiteMode.Lax});

    private static string CreateHash(string username, string password) =>
        HashingUtilities.GetSha512String($"{Salt}:{username}:{password}");
}
