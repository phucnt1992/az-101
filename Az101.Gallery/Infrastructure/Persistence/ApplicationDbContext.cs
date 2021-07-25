using Microsoft.EntityFrameworkCore;

using Az101.Gallery.Infrastructure.Persistence.Configurations;
using Az101.Gallery.Models;
using System.Diagnostics.CodeAnalysis;

namespace Az101.Gallery.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }

        public ApplicationDbContext([NotNull] DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PhotoTypeConfiguration());
        }

    }
}
