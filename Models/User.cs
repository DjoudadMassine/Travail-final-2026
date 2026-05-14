using DAL;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User : Record
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        
        [Required]
        public string AccessLevel { get; set; }

        public bool Verified { get; set; } = false;
    }
}