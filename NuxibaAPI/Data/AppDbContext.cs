using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Models;

namespace NuxibaAPI.Data;

// Contexto de base de datos principal
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tablas de la base de datos
    public DbSet<Login> Logins { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Area> Areas { get; set; }

    // Configuración de relaciones entre tablas
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Login → User (muchos a uno)
        modelBuilder.Entity<Login>()
            .HasOne(l => l.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(l => l.User_id);

        // User → Area (muchos a uno)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Area)
            .WithMany(a => a.Users)
            .HasForeignKey(u => u.IDArea);
    }
}
