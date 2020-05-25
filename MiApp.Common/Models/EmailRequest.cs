using System.ComponentModel.DataAnnotations;

namespace MiApp.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}