using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ERPSalesSystem.Models
{
    public class ERPContext : IdentityDbContext<ApplicationUser>
    {
        public ERPContext(DbContextOptions<ERPContext> options) : base(options) { }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
