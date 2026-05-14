using Models;
using System;
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
            return ToList().FirstOrDefault(l =>
                l.Email != null &&
                l.Email.ToLower() == email.ToLower()
            );
        }

        public bool EmailExists(string email)
        {
            return GetByEmail(email) != null;
        }

        public Login Verify(string email, string password)
        {
            return ToList().FirstOrDefault(l =>
                l.Email != null &&
                l.Email.ToLower() == email.ToLower()
                && l.Password == password
            );
        }

        public void DeleteByUserId(int userId)
        {
            var logins = ToList()
                .Where(l => l.UserId == userId)
                .ToList();

            foreach (var login in logins)
            {
                Delete(login.Id);
            }
        }

        public void AddLogin(int userId, string email)
        {
            Add(new Login
            {
                UserId = userId,
                Email = email,
                LoginDate = DateTime.Now
            });
        }
    }
}