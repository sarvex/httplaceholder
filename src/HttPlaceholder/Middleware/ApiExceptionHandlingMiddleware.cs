using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Middleware;

/// <summary>
///     A piece of middleware for handling exceptions.
/// </summary>
public class ApiExceptionHandlingMiddleware
{
    private readonly IHttpContextService _httpContextService;
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    /// <summary>
    ///     Constructs an <see cref="ApiExceptionHandlingMiddleware" /> instance.
    /// </summary>
    public ApiExceptionHandlingMiddleware(RequestDelegate next, IHttpContextService httpContextService,
        ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _httpContextService = httpContextService;
        _logger = logger;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (_httpContextService.Path?.Contains("ph-api/") == true)
        {
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            try
            {
                await _next(context);
            }
            catch (ConflictException)
            {
                _logger.LogDebug($"Handling {nameof(ConflictException)}.");
                _httpContextService.SetStatusCode(HttpStatusCode.Conflict);
            }
            catch (NotFoundException)
            {
                _logger.LogDebug($"Handling {nameof(NotFoundException)}.");
                _httpContextService.SetStatusCode(HttpStatusCode.NotFound);
            }
            catch (ForbiddenException)
            {
                _logger.LogDebug($"Handling {nameof(ForbiddenException)}.");
                _httpContextService.SetStatusCode(HttpStatusCode.Forbidden);
            }
            catch (ArgumentException ex)
            {
                _logger.LogDebug($"Handling {nameof(ArgumentException)}.");
                await WriteResponseBody(new[] {ex.Message}, HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (ValidationException ex)
            {
                _logger.LogDebug($"Handling {nameof(ValidationException)}.");
                await WriteResponseBody(ex.ValidationErrors, HttpStatusCode.BadRequest, cancellationToken);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task WriteResponseBody(object body, HttpStatusCode httpStatusCode,
        CancellationToken cancellationToken)
    {
        _httpContextService.SetStatusCode(httpStatusCode);
        _httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.JsonMime);
        var responseJson = JsonConvert.SerializeObject(body);
        await _httpContextService.WriteAsync(responseJson, cancellationToken);
        _logger.LogDebug($"Returning response body: {responseJson}");
    }
}
