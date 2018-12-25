﻿using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
    /// <summary>
    /// A model for storing all conditions for a stub.
    /// </summary>
    public class StubConditionsModel
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        [YamlMember(Alias = "method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [YamlMember(Alias = "url")]
        public StubUrlConditionModel Url { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [YamlMember(Alias = "body")]
        public IEnumerable<string> Body { get; set; }

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        [YamlMember(Alias = "form")]
        public IEnumerable<StubFormModel> Form { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [YamlMember(Alias = "headers")]
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the xpath.
        /// </summary>
        [YamlMember(Alias = "xpath")]
        public IEnumerable<StubXpathModel> Xpath { get; set; }

        /// <summary>
        /// Gets or sets the json path.
        /// </summary>
        [YamlMember(Alias = "jsonPath")]
        public IEnumerable<string> JsonPath { get; set; }

        /// <summary>
        /// Gets or sets the basic authentication.
        /// </summary>
        [YamlMember(Alias = "basicAuthentication")]
        public StubBasicAuthenticationModel BasicAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        [YamlMember(Alias = "clientIp")]
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [YamlMember(Alias = "host")]
        public string Host { get; set; }
    }
}