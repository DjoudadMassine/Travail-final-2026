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

        [Required]
        public string Password { get; set; }

        // Niveau d'accès
        public Access Access { get; set; } = Access.View;

        public bool Blocked { get; set; }

        public bool Verified { get; set; }

        public bool Notify { get; set; }

        public string Avatar { get; set; }

        public bool Online { get; set; }

        // Vérifie si l'utilisateur est admin
        [JsonIgnore]
        public bool IsAdmin
        {
            get
            {
                return Access == Access.Admin;
            }
        }

        // Vérifie si l'utilisateur peut écrire/modifier
        [JsonIgnore]
        public bool CanWrite
        {
            get
            {
                return Access == Access.Write
                    || Access == Access.Admin;
            }
        }

        // Vérifie si utilisateur peut seulement voir
        [JsonIgnore]
        public bool CanView
        {
            get
            {
                return Access >= Access.View;
            }
        }

        // Vérifie si compte utilisable
        [JsonIgnore]
        public bool IsAvailable
        {
            get
            {
                return !Blocked && Verified;
            }
        }
    }
}