using System.ComponentModel.DataAnnotations;

namespace apiCalendar.Models
{
    public class ValidateRequest
    {
        [Required]
        public string? token { get; set; }
        [Required]
        public string? aplicationId { get; set; }
        [Required]
        public string? endPoint { get; set; }
        [Required]
        public string? httpMethod { get; set; }
    }
}
