using System;
using System.Security.Cryptography;
using System.Text;

namespace LifeOS.Helper
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash password using SHA256
        /// Note: For production, use BCrypt, PBKDF2, or Argon2
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Verify password against stored hash
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            // For backward compatibility, also check plain text (development only)
            if (password == storedHash)
                return true;

            string hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
