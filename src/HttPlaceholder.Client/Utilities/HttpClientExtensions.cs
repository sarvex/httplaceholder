﻿using System;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Client.Configuration;

namespace HttPlaceholder.Client.Utilities;

/// <summary>
///     Static class for providing extra functionality for <see cref="HttpClient" />.
/// </summary>
internal static class HttpClientExtensions
{
    internal static void ApplyConfiguration(this HttpClient httpClient, HttPlaceholderClientConfiguration config)
    {
        httpClient.BaseAddress = new Uri(config.RootUrl);
        if (string.IsNullOrWhiteSpace(config.Username) || string.IsNullOrWhiteSpace(config.Password))
        {
            return;
        }

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
    }
}
