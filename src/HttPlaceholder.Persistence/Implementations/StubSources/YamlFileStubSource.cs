﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;
using Constants = HttPlaceholder.Domain.Constants;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
/// </summary>
internal class YamlFileStubSource : IStubSource
{
    private static readonly string[] _extensions = {".yml", ".yaml"};
    private readonly IFileService _fileService;
    private readonly ILogger<YamlFileStubSource> _logger;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly IStubModelValidator _stubModelValidator;
    private DateTime _stubLoadDateTime;

    private IEnumerable<StubModel> _stubs;

    public YamlFileStubSource(
        IFileService fileService,
        ILogger<YamlFileStubSource> logger,
        IOptionsMonitor<SettingsModel> options,
        IStubModelValidator stubModelValidator)
    {
        _fileService = fileService;
        _logger = logger;
        _stubModelValidator = stubModelValidator;
        _options = options;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken)
    {
        var fileLocations = (await GetYamlFileLocationsAsync(cancellationToken)).ToArray();
        if (fileLocations.Length == 0)
        {
            _logger.LogInformation("No .yml input files found.");
            return Array.Empty<StubModel>().AsEnumerable();
        }

        if (_stubs == null || GetLastStubFileModificationDateTime(fileLocations) > _stubLoadDateTime)
        {
            _stubs =
                (await Task.WhenAll(fileLocations.Select(l => LoadStubsAsync(l, cancellationToken))))
                .SelectMany(s => s);
        }
        else
        {
            _logger.LogDebug("No stub file contents changed in the meanwhile.");
        }

        return _stubs;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public async Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken)).FirstOrDefault(s => s.Id == stubId);

    /// <inheritdoc />
    public async Task PrepareStubSourceAsync(CancellationToken cancellationToken) =>
        // Check if the .yml files could be loaded.
        await GetStubsAsync(cancellationToken);

    private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files) =>
        files.Max(f => _fileService.GetLastWriteTime(f));

    private async Task<IEnumerable<StubModel>> LoadStubsAsync(string file, CancellationToken cancellationToken)
    {
        // Load the stubs.
        var input = await _fileService.ReadAllTextAsync(file, cancellationToken);
        _logger.LogInformation($"Parsing .yml file '{file}'.");
        try
        {
            var stubs = ParseAndValidateStubs(input, file);
            _stubLoadDateTime = DateTime.Now;
            return stubs;
        }
        catch (YamlException ex)
        {
            _logger.LogWarning(ex, $"Error occurred while parsing YAML file '{file}'");
        }

        return Array.Empty<StubModel>();
    }

    private IEnumerable<StubModel> ParseAndValidateStubs(string input, string file)
    {
        IEnumerable<StubModel> stubs;
        if (YamlIsArray(input))
        {
            stubs = YamlUtilities.Parse<List<StubModel>>(input);
        }
        else
        {
            stubs = new[] {YamlUtilities.Parse<StubModel>(input)};
        }

        return ValidateStubs(file, stubs);
    }

    private IEnumerable<StubModel> ValidateStubs(string filename, IEnumerable<StubModel> stubs)
    {
        var result = new List<StubModel>();
        foreach (var stub in stubs)
        {
            if (string.IsNullOrWhiteSpace(stub.Id))
            {
                // If no ID is set, log a warning as the stub is invalid.
                _logger.LogWarning($"Stub in file '{filename}' has no 'id' field defined, so is not a valid stub.");
                continue;
            }

            // Right now, stubs loaded from YAML files are allowed to have validation errors.
            // They are NOT allowed to have no ID however.
            var validationResults = _stubModelValidator.ValidateStubModel(stub).ToArray();
            if (validationResults.Any())
            {
                validationResults = validationResults.Select(r => $"- {r}").ToArray();
                _logger.LogWarning(
                    $"Validation warnings encountered for stub '{stub.Id}':\n{string.Join("\n", validationResults)}");
            }

            result.Add(stub);
        }

        return result;
    }

    private static string StripIllegalCharacters(string input) => input.Replace("\"", string.Empty);

    private static bool YamlIsArray(string yaml) => yaml
        .SplitNewlines()
        .Any(l => l.StartsWith("-"));

    private async Task<IEnumerable<string>> GetYamlFileLocationsAsync(CancellationToken cancellationToken)
    {
        var inputFileLocation = _options.CurrentValue.Storage?.InputFile;
        if (string.IsNullOrEmpty(inputFileLocation))
        {
            // If the input file location is not set, try looking in the current directory for .yml files.
            var currentDirectory = _fileService.GetCurrentDirectory();
            return await _fileService.GetFilesAsync(currentDirectory, _extensions, cancellationToken);
        }

        // Split file path: it is possible to supply multiple locations.
        var locations = inputFileLocation
            .Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(StripIllegalCharacters);
        return (await Task.WhenAll(locations.Select(l => ParseFileLocationsAsync(l, cancellationToken))))
            .SelectMany(p => p);
    }

    private async Task<IEnumerable<string>> ParseFileLocationsAsync(string part, CancellationToken cancellationToken)
    {
        var location = part.Trim();
        _logger.LogInformation($"Reading location '{location}'.");
        if (await _fileService.IsDirectoryAsync(location, cancellationToken))
        {
            return await _fileService.GetFilesAsync(location, _extensions, cancellationToken);
        }

        return new[] {location};
    }
}
