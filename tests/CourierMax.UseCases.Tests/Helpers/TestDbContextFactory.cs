using CourierMax.Data;
using Microsoft.EntityFrameworkCore;

namespace CourierMax.UseCases.Tests.Helpers;

/// <summary>
/// IDbContextFactory<CourierMaxContext> backed by an EF Core InMemory database.
/// Each instance uses a unique database name so tests are fully isolated.
/// </summary>
public sealed class TestDbContextFactory : IDbContextFactory<CourierMaxContext>
{
    private readonly DbContextOptions<CourierMaxContext> _options;

    public TestDbContextFactory()
    {
        var dbName = $"CourierMaxTest_{Guid.NewGuid():N}";
        _options = new DbContextOptionsBuilder<CourierMaxContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    public CourierMaxContext CreateDbContext() => new(_options);

    public Task<CourierMaxContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());
}
