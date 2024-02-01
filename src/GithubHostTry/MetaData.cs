using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GithubHostTry
{
    public class MetaData
    {
        [JsonPropertyName("verifiable_password_authentication")]
        public bool Verifiale { get; set; }
        [JsonPropertyName("hooks")]
        public string[]? Hooks { get; set; }
        [JsonPropertyName("web")]
        public string[]? Webs { get; set; }
        [JsonPropertyName("api")]
        public string[]? Apis { get; set; }
        [JsonPropertyName("git")]
        public string[]? Gits { get; set; }
        [JsonPropertyName("packages")]
        public string[]? Packages { get; set; }
        [JsonPropertyName("pages")]
        public string[]? Pages { get; set; }
    }
}
