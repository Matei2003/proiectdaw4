using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proiectdaw4.Data;
using proiectdaw4.Model;

namespace proiectdaw4.Services
{
    public class AuthenticationService
    {
        private readonly BdContext _context;

        public AuthenticationService(BdContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if(user == null || user.Password != password)
            {
                return null;
            }

            return user;

        }
    }
}
