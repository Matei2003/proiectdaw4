using Microsoft.EntityFrameworkCore;
using proiectdaw4.Model;

namespace proiectdaw4.Data
{
    public class BdContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Proprietate> Proprietati { get; set; }

        public DbSet<Inchiriere> Inchirieri { get; set; }


        public BdContext(DbContextOptions<BdContext> options) : base(options)
        {

        }
    }
}
