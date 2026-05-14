using DAL;

namespace Models
{
    public class Login : Record
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}