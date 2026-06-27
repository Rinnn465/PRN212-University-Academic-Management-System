using Microsoft.EntityFrameworkCore;

namespace DAL;

public static class AppDbContextFactory
{
    public const string DefaultConnectionString =
        "Server=localhost\\SQLEXPRESS;Database=StudentManagementDB;User Id=sa;Password=12345;Encrypt=False;TrustServerCertificate=True";

    public static AppDbContext Create(string? connectionString = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(string.IsNullOrWhiteSpace(connectionString) ? DefaultConnectionString : connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
