using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Models
{
    public class CommentsRepository : Repository<Comment>
    {
        public void ToggleComment(int parentId, int userId)
        {
            Commentlike like = DB.Commentlikes.ToList().FirstOrDefault(l => l.CommentId == parentId && l.UserId == userId);
            Comment comment = DB.Comments.Get(parentId);

            if (like != null)
            {
                BeginTransaction();
                DB.Commentlikes.Delete(like.Id);
                EndTransaction();
            }
            else
            {
                BeginTransaction();
                DB.Commentlikes.Add(new Commentlike
                {
                    CommentId = parentId,
                    UserId = userId,
                });
                EndTransaction();
            }
        }

        public void DeleteByOwnerId(int ownerId)
        {
            List<Comment> list = ToList().Where(p => p.OwnerId == ownerId).ToList();
            foreach (var comment in list)
            {
                DeleteAllComments(comment.Id);
            }
        }
        public override int Add(Comment data)
        {
            Comment comment = DB.Comments.Get(base.Add(data));
            comment.Media?.ResetCountsCalc();
            return comment.Id;
        }

        public override bool Delete(int commentId)
        {
            try
            {
                Comment commentToDelete = DB.Comments.Get(commentId);
                if (commentToDelete != null)
                {
                    Media relatedMedia = commentToDelete.Media;
                    BeginTransaction();
                    DeleteAllComments(commentId);
                    base.Delete(commentId);
                    EndTransaction();
                    relatedMedia.ResetCountsCalc();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Delete comment failed : Message - {ex.Message}");
                EndTransaction();
                return false;
            }
        }

        public void DeleteCommentByMediaId(int mediaId)
        {
            List<Comment> list = ToList().Where(l => l.MediaId == mediaId && l.ParentId == 0).ToList();
            foreach (var comment in list)
            {
                DeleteAllComments(comment.Id);
            }
        }

        public void DeleteAllComments(int commentId)
        {
            DB.Commentlikes.DeleteByCommentId(commentId);

            List<Comment> enfants = ToList().Where(c => c.ParentId == commentId).ToList();
            foreach (var enfant in enfants)
            {
                DeleteAllComments(enfant.Id);
            }
            base.Delete(commentId);
        }
    }

}
