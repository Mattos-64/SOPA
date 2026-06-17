using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SOPA.Core.Models;

namespace SOPA.Api.Data;

public class AppDbContext : IdentityDbContext<Voluntario, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Animal> Animais { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Animal>().ToTable("Animais");
        builder.Entity<Animal>().Property(a => a.Nome).HasMaxLength(100).IsRequired();
    }
}