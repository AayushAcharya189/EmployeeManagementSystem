using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Models
{
    public class EmployeeDbContext : IdentityDbContext<ApplicationUser>
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);     // Calling base.OnModelCreating(builder) to 
                                               // ensures Identity tables are configured correctly
            builder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(10, 2);
        }
    }
}
