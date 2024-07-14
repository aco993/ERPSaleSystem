using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ERPSalesSystem.Models
{
    public class ERPContext : IdentityDbContext<ApplicationUser>
    {
        public ERPContext(DbContextOptions<ERPContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }
}
