using System.Text.Json.Serialization;

namespace Skrawl.API.Data.Models
{
    public class ImpersonationRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
