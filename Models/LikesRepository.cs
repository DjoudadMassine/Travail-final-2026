using DAL;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class LikesRepository : Repository<Like>
    {
        public void ToggleLike(int mediaId, int userId)
        {
            Like like = ToList().Where(l => (l.MediaId == mediaId && l.UserId == userId)).FirstOrDefault();

            if (like != null)
            {
                DB.Notifications.Push(like.Media.OwnerId, like.User.Name + " n'aime plus votre vidéo \n[" + like.Media.Title + "]");
                Delete(like.Id);
            }
            else
            {
                like = new Like { MediaId = mediaId, UserId = userId };
                DB.Notifications.Push(like.Media.OwnerId, like.User.Name + " aime votre vidéo \n[" + like.Media.Title + "]");
                Add(like);
            }
        }
        public void DeleteByMediaId(int mediaId)
        {
            List<Like> list = ToList().Where(l => l.MediaId == mediaId).ToList().Copy();
            list.ForEach(l => Delete(l.Id));
        }
        public void DeleteByUserId(int userId)
        {
            List<Like> list = ToList().Where(l => l.UserId == userId).ToList().Copy();
            list.ForEach(l => Delete(l.Id));
        }
    }
}