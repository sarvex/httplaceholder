﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert a given query parameter in the response.
/// </summary>
internal class QueryStringResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public QueryStringResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) :
        base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "query";

    /// <inheritdoc />
    public override string FullName => "Query string";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}:query_string_key))"};

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var queryDict = _httpContextService.GetQueryStringDictionary();
        return Task.FromResult(matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input, (current, match) => InsertQuery(current, match, queryDict)));
    }

    private static string InsertQuery(string current, Match match, IDictionary<string, string> queryDict)
    {
        var queryStringName = match.Groups[2].Value;
        if (!queryDict.TryGetValue(queryStringName, out var replaceValue))
        {
            replaceValue = string.Empty;
        }

        return current.Replace(match.Value, replaceValue);
    }
}
