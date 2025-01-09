using Kafejka.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kafejka.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitItem> VisitItems { get; set; }
        public DbSet<Stamp> Stamps { get; set; }
        public DbSet<Reward> Rewards { get; set; }

        //O tej metodzie bym musial poczytac poki co jej nie uzywam

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

            // Przykładowe dodatkowe konfiguracje:
         //   modelBuilder.Entity<MenuItem>()
         //       .Property(m => m.Category)
        //        .HasConversion<string>(); // Konwersja dla enum, jeśli będzie używane.
       // }
    }
}
