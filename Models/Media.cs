using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Models
{
    public enum MediaSortBy { Title, PublishDate, Likes, Comments }

    public class Media : Record
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string YoutubeId { get; set; }

        public int OwnerId { get; set; } = 1;
        public bool Shared { get; set; } = true;
        public DateTime PublishDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId).Copy();
                
        [JsonIgnore]
        private int _likesCount = -1;
        [JsonIgnore]
        private List<Like> _likesList = null;
        [JsonIgnore]
        private int _commentsCount = -1;
        [JsonIgnore]
        private List<Comment> _commentsList = null;
        public void ResetCountsCalc()
        {
            _likesCount = -1;
            _likesList = null;
            _commentsCount = -1;
            _commentsList = null;
        }
        [JsonIgnore]
        public int LikesCount
        {
            get
            {
                if (_likesCount == -1)
                    _likesCount = Likes.Count();
                return _likesCount;
            }
        }
        [JsonIgnore]
        public List<Like> Likes
        {
            get
            {
                if (_likesList == null)
                    _likesList = DB.Likes.ToList().Where(l => l.MediaId == Id).ToList();
                return _likesList;
            }
        }

        [JsonIgnore]
        public string UsersLikesList
        {
            get
            {
                string UsersLikesList = "Usagers qui ont aimé :" + "\n";
                foreach (var like in Likes)
                {
                    User user = DB.Users.Get(like.UserId);
                    if (user != null) 
                        UsersLikesList += user.Name + "\n";
                }
                return UsersLikesList;
            }
        }
        public bool DeleteLikes()
        {
            List<Like> likes = DB.Likes.ToList().Where(l => l.MediaId == this.Id).ToList();
            likes.Copy().ForEach(m => DB.Likes.Delete(m.Id));
            return true;
        }

        [JsonIgnore]
        public string UsersCommentList
        {
            get
            {
                string UsersCommentList = "Usagers qui ont commenté :" + "\n";
                foreach (var comment in Comments)
                {
                    string name = DB.Users.Get(comment.OwnerId).Name;
                    if (!UsersCommentList.Contains(name))
                        UsersCommentList += DB.Users.Get(comment.OwnerId).Name + "\n";
                }
                return UsersCommentList;
            }
        }
        [JsonIgnore]
        public int CommentsCount
        {
            get
            {
                if (_commentsCount == -1)
                    _commentsCount = Comments.Count();
                return _commentsCount;
            }
        }
        [JsonIgnore]
        public List<Comment> Comments
        {
            get
            {
                if (_commentsList == null)
                    _commentsList = DB.Comments.ToList().Where(c => c.MediaId == Id).ToList();
                return _commentsList;
            }
        }
    }
}