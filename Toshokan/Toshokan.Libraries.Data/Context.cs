using Microsoft.EntityFrameworkCore;
using Toshokan.Libraries.Models;

namespace Toshokan.Libraries.Data
{
    public class Context : DbContext
    {
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
    }
}