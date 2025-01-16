using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kafejka.Models;

namespace Kafejka.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Kafejka.Models.ItemType> ItemType { get; set; } = default!;
        public DbSet<Kafejka.Models.MenuItem> MenuItem { get; set; } = default!;
        public DbSet<Kafejka.Models.Transaction> Transaction { get; set; } = default!;
        public DbSet<Kafejka.Models.TransactionItemsList> TransactionItemsList { get; set; } = default!;
    }
}
