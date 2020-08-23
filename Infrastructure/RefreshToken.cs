using System;
using System.Text.Json.Serialization;

namespace Skrawl.API.Infrastructure
{
    public class RefreshToken
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }    // can be used for usage tracking
        // can optionally include other metadata, such as user agent, ip address, device name, and so on

        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}