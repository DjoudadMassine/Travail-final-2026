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

        // Comptes / sécurité
        static public Repository<User> Users { get; set; }
            = new Repository<User>();

        static public LoginsRepository Logins { get; set; }
            = new LoginsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();

        // PFI Programme 420
        static public Repository<Student> Students { get; set; }
            = new Repository<Student>();

        static public Repository<Course> Courses { get; set; }
            = new Repository<Course>();

        static public Repository<Teacher> Teachers { get; set; }
            = new Repository<Teacher>();

        static public Repository<Registration> Registrations { get; set; }
            = new Repository<Registration>();

        static public Repository<Allocation> Allocations { get; set; }
            = new Repository<Allocation>();
    }
}