using Microsoft.EntityFrameworkCore;
using RH_Backend.Models;

namespace RH_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Ferias> Ferias { get; set; } = null!;
        public DbSet<HistoricoFuncionario> HistoricoFuncionarios { get; set; }
    }
}
