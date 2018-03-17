using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using campgrounds_api.Models;
using Microsoft.EntityFrameworkCore;

namespace campgrounds_api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return null;

            return VerifyPassword(password, user.PasswordHash, user.PasswordSalt) ? user : null;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.ASCII.GetBytes(password));
                return CheckHash(passwordHash, computedHash);
            }
        }

        private bool CheckHash(byte[] passwordHash, byte[] computedHash)
        {
            if (passwordHash.Length != computedHash.Length)
                return false;

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != computedHash[i])
                    return false;
            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.ASCII.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(u => u.UserName == username);
        }
    }
}