using System;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            this._context = context;            
        }
        public async Task<User> Login(string userName, string password)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if(user == null)
                return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var pHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < pHash.Length; i++)
                    if(pHash[i] != passwordHash[i])
                        return false;
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] pHash, pSalt;
            CreatePasswordHash(password, out pHash, out pSalt);
            user.PasswordHash = pHash;
            user.PasswordSalt = pSalt;
            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();
            return user;
        }
        private void CreatePasswordHash(string password, out byte[] pHash, out byte[] pSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pSalt = hmac.Key;
                pHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if(!await this._context.Users.AnyAsync(u => u.UserName == userName))
                return true;
            return false;
        }
    }
}