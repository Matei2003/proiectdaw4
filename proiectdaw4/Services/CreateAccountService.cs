using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proiectdaw4.Data;
using proiectdaw4.Model;
using System.Numerics;

namespace proiectdaw4.Services
{
    public class CreateAccountService
    {

        private readonly BdContext _context;

        public CreateAccountService(BdContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage)> Register(string firstName, string lastName, string phone, string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                return (false, "Acest mail este deja folosit");
            }

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                PhoneNumber = phone
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return (true, null);
        }

    }
}