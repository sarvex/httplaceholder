using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler to insert the local date/time into the response. An optional date/time format can
///     be provided (based on the .NET date/time formatting strings).
/// </summary>
internal class LocalNowResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IDateTime _dateTime;

    public LocalNowResponseVariableParsingHandler(IDateTime dateTime, IFileService fileService) : base(fileService)
    {
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public override string Name => "localnow";

    /// <inheritdoc />
    public override string FullName => "Local date / time";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))", $"(({Name}:yyyy-MM-dd HH:mm:ss))"};

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var now = _dateTime.Now;
        return Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => InsertDateTime(current, match, now)));
    }

    private static string InsertDateTime(string current, Match match, DateTime now)
    {
        var dateTime = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
            ? now.ToString(match.Groups[2].Value)
            : now.ToString(CultureInfo.InvariantCulture);
        return current.Replace(match.Value, dateTime);
    }
}
