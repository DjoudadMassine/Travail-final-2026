using DAL;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum Access
    {
        Anonymous = 0,
        View = 1,
        Write = 2,
        Admin = 3
    }

    public class User : Record
    {
        [JsonIgnore]
        public static User ConnectedUser { get; set; }

        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public Access Access { get; set; }

        public bool Blocked { get; set; }

        public bool Verified { get; set; }

        public bool Notify { get; set; }

        public string Avatar { get; set; }

        public bool Online { get; set; }

        [JsonIgnore]
        public bool IsAdmin
        {
            get { return Access == Access.Admin; }
        }

        [JsonIgnore]
        public bool CanWrite
        {
            get { return Access == Access.Write || Access == Access.Admin; }
        }
    }
}