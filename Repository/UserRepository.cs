using AuthApp.Data;
using AuthApp.Interfaces;
using AuthApp.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                return false;

            if(!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt)) return false;

            string newPasswordHash, newPasswordSalt;
            CreatePasswordHash(newPassword, out newPasswordHash, out newPasswordSalt);

            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;

            
        }

        public async Task<User> Register(User user, string password)
        {
            string passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;  

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<object>> SearchUsers(string searchTerm)
        {
            return await _context.Users.Where(u => u.Username.Contains(searchTerm)).Select(u => new
            {
                u.id,
                u.Username,
                u.FirstName,
                u.LastName,
                u.Email,
            }).ToListAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return true;

            return false;
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();

            passwordHash = BCrypt.Net.BCrypt.HashPassword(password, passwordSalt);
        }


        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
