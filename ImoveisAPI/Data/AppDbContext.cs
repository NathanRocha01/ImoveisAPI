using ImoveisAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImoveisAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Proprietario> Proprietarios { get; set; }
        public DbSet<Imovel> Imoveis { get; set; }
        public DbSet<Cadastro> Cadastros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Proprietario>()
                .HasIndex(p => p.Documento)
                .IsUnique();

            modelBuilder.Entity<Imovel>()
                .HasOne(i => i.Proprietario)
                .WithMany(p => p.Imoveis)
                .HasForeignKey(i => i.ProprietarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cadastro>()
                .HasOne(c => c.Proprietario)
                .WithMany()
                .HasForeignKey(c => c.ProprietarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

