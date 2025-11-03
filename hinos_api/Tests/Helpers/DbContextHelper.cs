using Microsoft.EntityFrameworkCore;
using hinos_api.Data;

namespace hinos_api.Tests.Helpers;

public static class DbContextHelper
{
    public static HymnsDbContext CreateInMemoryDbContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<HymnsDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new HymnsDbContext(options);
    }
}
