
using System;
using System.Threading.Tasks;

using GHARSA.API.Data;
using GHARSA.API.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        private readonly IPasswordHasher<User>
            _passwordHasher;

        public UserService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET USER BY ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    u => u.UserId == id
                );
        }

        // LOGIN / AUTHENTICATE
        public async Task<User?> AuthenticateAsync(
            string email,
            string password)
        {
            string normalizedEmail =
                email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email.ToLower()
                         == normalizedEmail
                );

            if (user == null)
                return null;

            var result = _passwordHasher
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password
                );

            if (result == PasswordVerificationResult.Failed)
                return null;

            if (result ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(
                        user,
                        password
                    );

                await _context.SaveChangesAsync();
            }

            return user;
        }

        // REGISTER NEW USER
        public async Task<User?> RegisterAsync(
            string fullName,
            string email,
            string password)
        {
            string normalizedEmail =
                email.Trim().ToLowerInvariant();

            bool exists = await _context.Users
                .AnyAsync(
                    u => u.Email.ToLower()
                         == normalizedEmail
                );

            if (exists)
                return null;

            var user = new User
            {
                FullName = fullName.Trim(),
                Email = normalizedEmail,
                CreatedAt = DateTime.UtcNow,

                // All public registrations are Users
                Role = "User"
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    password
                );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        // UPDATE PROFILE NAME
        public async Task<bool> UpdateProfileAsync(
            int userId,
            string fullName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.UserId == userId
                );

            if (user == null)
                return false;

            user.FullName = fullName.Trim();

            await _context.SaveChangesAsync();

            return true;
        }
    }
}