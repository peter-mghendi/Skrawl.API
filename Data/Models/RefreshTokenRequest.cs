using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Skrawl.API.Data.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
