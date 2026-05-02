using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

namespace Models
{
    public class Comment : Record
    {
        public int OwnerId { get; set; } = 0;
        public int MediaId { get; set; } = 0;
        public int ParentId { get; set; } = 0;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string Text { get; set; }

        [JsonIgnore]
        public List<Commentlike> Likes => DB.Commentlikes.ToList().Where(l => l.CommentId == Id).ToList();

        [JsonIgnore]
        public string UsersLikesList
        {
            get
            {
                string UsersLikesList = "";
                foreach (var like in Likes)
                {
                    UsersLikesList += DB.Users.Get(like.UserId).Name + "\n";
                }
                return UsersLikesList;
            }
        }
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId);
        [JsonIgnore]
        public Media Media => DB.Medias.Get(MediaId);
    }
}