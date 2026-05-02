using Newtonsoft.Json;
using System;
using DAL;

namespace Models
{
    public class Commentlike : Record
    {
        public int UserId { get; set; }
        public int CommentId { get; set; } = 0;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public User User => DB.Users.Get(UserId);
        [JsonIgnore]
        public Comment Comment => DB.Comments.Get(CommentId);
    }
}