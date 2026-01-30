using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Okane.Storage.EntityFramework;

public sealed class OkaneDbContextFactory
    : IDesignTimeDbContextFactory<OkaneDbContext>
{
    public OkaneDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OkaneDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=OkaneLocal;Username=postgres;Password=Elior_r2316@");

        return new OkaneDbContext(optionsBuilder.Options);
    }
}