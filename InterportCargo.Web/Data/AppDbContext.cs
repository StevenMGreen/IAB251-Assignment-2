using InterportCargo.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<EmployeeAccount> EmployeeAccounts => Set<EmployeeAccount>();
    public DbSet<QuotationRequest> QuotationRequests => Set<QuotationRequest>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<EmployeeAccount>()
            .HasIndex(e => e.Email)
            .IsUnique();
    }
}
