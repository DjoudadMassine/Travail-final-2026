using EmailHandling;
using Models;

namespace DAL
{
    public sealed class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance { get { return instance; } }
        #endregion

        static public UsersRepository Users { get; set; }
            = new UsersRepository();

        static public NotificationsRepository Notifications { get; set; }
            = new NotificationsRepository();

        static public LoginsRepository Logins { get; set; }
            = new LoginsRepository();

        static public EventsRepository Events { get; set; }
            = new EventsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();

        static public MediasRepository Medias { get; set; }
            = new MediasRepository();

        static public LikesRepository Likes { get; set; }
           = new LikesRepository();

        static public CommentLikesRepository Commentlikes { get; set; }
            = new CommentLikesRepository();

        static public CommentsRepository Comments { get; set; }
           = new CommentsRepository();
    }
}