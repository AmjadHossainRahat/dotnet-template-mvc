using Application.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        // Uses HMACSHA512 for hashing with per-user salt
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            using var hmac = new HMACSHA512();
            passwordSalt = Encoding.UTF8.GetString(hmac.Key);
            passwordHash = Encoding.UTF8.GetString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (storedHash == null || storedHash.Length == 0) throw new ArgumentException("Invalid stored hash", nameof(storedHash));
            if (storedSalt == null || storedSalt.Length == 0) throw new ArgumentException("Invalid stored salt", nameof(storedSalt));

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(storedSalt));
            var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            if (computed.Length != storedHash.Length) return false;

            for (int i = 0; i < computed.Length; i++)
                if (computed[i] != storedHash[i]) return false;

            return true;
        }
    }
}
