using DAL;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class CommentLikesRepository : Repository<Commentlike>
    {
        public void ToggleLike(int commentId, int userId)
        {
            Commentlike like = ToList().Where(l => (l.CommentId == commentId && l.UserId == userId)).FirstOrDefault();

            if (like != null)
            {
                DB.Notifications.Push(like.UserId, like.User.Name + " n'aime plus votre commentaire \n[" + like.Comment.Text + "]");
                Delete(like.Id);
            }
            else
            {
                like = new Commentlike {CommentId = commentId, UserId = userId };
                DB.Notifications.Push(like.UserId, like.User.Name + " aime votre commentaire \n[" + like.Comment.Text + "]");
                Add(like);
            }
        }
       
        public void DeleteByUserId(int userId)
        {
            List<Commentlike> list = ToList().Where(l => l.UserId == userId).ToList().Copy();
            list.ForEach(l => Delete(l.Id));
        }

        public void DeleteByCommentId(int commentId)
        {
            List<Commentlike> list = ToList().Where(l => l.CommentId == commentId).ToList();
            list.ForEach(l => Delete(l.Id));
        }
    }
}