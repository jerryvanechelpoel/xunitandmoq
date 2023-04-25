using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Ird.XunitAndMoq.Testing.EfCore;

public static class TestDbContextFactory
{
    public static async Task<TDbContext> CreateContextAsync<TDbContext>(Func<DbContextOptions, TDbContext> createContextMethod,
                                                                        Action<TDbContext> seedMethod = default,
                                                                        CancellationToken cancellationToken = default)
        where TDbContext : DbContext
    {
        DbContextOptions options = new DbContextOptionsBuilder<TDbContext>()
                                                .UseSqlite(CreateInMemoryDatabase())
                                                .Options;
        TDbContext context = createContextMethod(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        seedMethod?.Invoke(context);

        await context.SaveChangesAsync(cancellationToken);

        return context;
    }

    private static DbConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");

        connection.Open();

        return connection;
    }
}
