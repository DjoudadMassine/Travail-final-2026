using Newtonsoft.Json;
using System;
using DAL;

namespace Models
{
    public class Like :Record
    {
        public int UserId { get; set; }
        public int MediaId { get; set; } = 0;
        //public int CommentId { get; set; } = 0;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public User User => DB.Users.Get(UserId);
        [JsonIgnore]
        public Media Media => DB.Medias.Get(MediaId);
    }
}