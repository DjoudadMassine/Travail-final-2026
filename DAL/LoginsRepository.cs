using Models;
using System.Linq;

namespace DAL
{
    public class LoginsRepository : Repository<Login>
    {
        public Login GetByUserId(int userId)
        {
            return ToList().FirstOrDefault(l => l.UserId == userId);
        }

        public Login GetByEmail(string email)
        {
            return ToList().FirstOrDefault(l => l.Email.ToLower() == email.ToLower());
        }

        public bool EmailExists(string email)
        {
            return GetByEmail(email) != null;
        }

        public Login Verify(string email, string password)
        {
            return ToList().FirstOrDefault(l =>
                l.Email.ToLower() == email.ToLower()
                && l.Password == password
            );
        }
    }
}