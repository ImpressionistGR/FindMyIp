namespace FindMyIp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using FindMyIp.Entities;
using FindMyIp.Domain.Dto;

/// <summary>
///
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="options"></param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    /// <summary>
    ///
    /// </summary>
    public DbSet<Country> Countries { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DbSet<IpAddress> IpAddresses { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DbSet<CountryReport> CountryReports { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);
            entity.Property(e => e.TwoLetterCode)
            .IsRequired()
            .HasMaxLength(2);
            entity.Property(e => e.ThreeLetterCode)
            .IsRequired()
            .HasMaxLength(3);
            entity.Property(e => e.CreatedAt)
            .IsRequired();
            entity.HasIndex(e => e.TwoLetterCode)
            .IsClustered(false)
            .IsUnique();
        });

        modelBuilder.Entity<IpAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            entity.Property(e => e.Ip)
            .IsRequired()
            .HasMaxLength(15);
            entity.Property(e => e.CreatedAt)
            .IsRequired();
            entity.Property(e => e.UpdatedAt)
            .IsRequired();
            entity.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId);
            entity.HasIndex(e => e.Ip)
            .IsClustered(false)
            .IsUnique();
        });

        modelBuilder.Entity<CountryReport>()
        .HasNoKey()
        .ToView(null)
        .HasNoDiscriminator()
        .ToSqlQuery("EXEC GetCountryReport @CountryCodes");
    }
}
