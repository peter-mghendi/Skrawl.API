using System.Text.Json.Serialization;

namespace Skrawl.API.Data.Models
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
