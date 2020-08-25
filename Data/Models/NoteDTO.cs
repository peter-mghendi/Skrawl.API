using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Skrawl.API.Data.Models
{
    public class NoteDTO
    {
        [Required]
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}