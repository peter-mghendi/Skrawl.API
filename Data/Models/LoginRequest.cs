using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Skrawl.API.Data.Models
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
