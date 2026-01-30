using Microsoft.EntityFrameworkCore;
using Okane.Application;

namespace Okane.Storage.EntityFramework;

public class OkaneDbContext(DbContextOptions<OkaneDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OkaneDbContext).Assembly);

        modelBuilder.Entity<Expense>()
            .HasOne(expense => expense.Category)
            .WithMany(category => category.Expenses)
            .HasForeignKey(expense => expense.CategoryId);
    }
};
