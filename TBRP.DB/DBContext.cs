using Microsoft.EntityFrameworkCore;

namespace TBRP.DB;

public class TbrpContext : DbContext
{
    public DbSet<Punishment> Punishments { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        optionsBuilder.UseNpgsql($"Host={host};" +
                                 $"Port={port};" +
                                 $"Database={database};" +
                                 $"Username={user};" +
                                 $"Password={pass};");
    }
}