<<<<<<< HEAD
using BackEnd_Synthetis.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Synthetis.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Modelo> Modelos => Set<Modelo>();

    public DbSet<Relatorio> Relatorios => Set<Relatorio>();
=======
using BackEnd_Synthetis.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Synthetis.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Modelo> Modelos => Set<Modelo>();

    public DbSet<Relatorio> Relatorios => Set<Relatorio>();
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}