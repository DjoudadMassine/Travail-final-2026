using DAL;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Models
{
    public class UsersRepository : Repository<User>
    {
        #region Password Encryption

        const int SaltSize = 20;

        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            randomNumberGenerator.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        private static string HashPassword(string password, string salt = "")
        {
            if (string.IsNullOrEmpty(salt))
                salt = CreateSalt(SaltSize);

            string saltedPassword = password + salt;

            HashAlgorithm encryptAlgorithm = new SHA256CryptoServiceProvider();
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(saltedPassword);
            byte[] bytHash = encryptAlgorithm.ComputeHash(bytValue);

            string base64 = Convert.ToBase64String(bytHash);

            return base64 + salt;
        }

        private static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
                return false;

            string salt = storedPassword.Substring(storedPassword.Length - CreateSalt(SaltSize).Length);
            string hashedPassword = HashPassword(password, salt);

            return hashedPassword == storedPassword;
        }

        #endregion

        public bool EmailExist(string email)
        {
            return ToList()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower()) != null;
        }

        public User GetUser(LoginCredential credential)
        {
            if (credential == null)
                return null;

            if (credential.Email == null || credential.Password == null)
                return null;

            string email = credential.Email.Trim().ToLower();
            string password = credential.Password.Trim();

            User user = ToList()
                .FirstOrDefault(u => u.Email.ToLower() == email);

            if (user != null && VerifyPassword(password, user.Password))
                return user.Copy();

            return null;
        }

        public override int Add(User user)
        {
            user.Password = HashPassword(user.Password);
            return base.Add(user);
        }

        public override bool Update(User user)
        {
            User storedUser = Get(user.Id);

            if (storedUser != null && user.Password != storedUser.Password)
                user.Password = HashPassword(user.Password);

            return base.Update(user);
        }

        public bool ChangePassword(User user)
        {
            user.Password = HashPassword(user.Password);
            return base.Update(user);
        }

        public override bool Delete(int userId)
        {
            try
            {
                User userToDelete = Get(userId);

                if (userToDelete != null)
                {
                    BeginTransaction();

                    DB.Logins.DeleteByUserId(userId);

                    base.Delete(userId);

                    EndTransaction();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Remove user failed : " + ex.Message);
                EndTransaction();
                return false;
            }
        }
    }
}