using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data.Models;
using RealEstateCMS.Data.Enums;

namespace RealEstateCMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Development> Developments => Set<Development>();
        public DbSet<Phase> Phases => Set<Phase>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<HouseType> HouseTypes => Set<HouseType>();
        public DbSet<Plot> Plots => Set<Plot>();
        public DbSet<Image> Images => Set<Image>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* =========================
             * Development
             * ========================= */
            modelBuilder.Entity<Development>(entity =>
            {
                entity.HasKey(e => e.DevelopmentId);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("datetime('now')");
            });

            /* =========================
             * Phase
             * ========================= */
            modelBuilder.Entity<Phase>(entity =>
            {
                entity.HasKey(e => e.PhaseId);
                entity.HasIndex(e => new { e.DevelopmentId, e.Name }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Development)
                      .WithMany(d => d.Phases)
                      .HasForeignKey(e => e.DevelopmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
             * Building
             * ========================= */
            modelBuilder.Entity<Building>(entity =>
            {
                entity.HasKey(e => e.BuildingId);
                entity.HasIndex(e => new { e.PhaseId, e.Name }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Phase)
                      .WithMany(p => p.Buildings)
                      .HasForeignKey(e => e.PhaseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
             * HouseType
             * ========================= */
            modelBuilder.Entity<HouseType>(entity =>
            {
                entity.HasKey(e => e.HouseTypeId);
                entity.HasIndex(e => new { e.PhaseId, e.Name }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FromPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PropertyType).HasConversion<string>();

                entity.HasOne(e => e.Phase)
                      .WithMany(p => p.HouseTypes)
                      .HasForeignKey(e => e.PhaseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
             * Plot
             * ========================= */
            modelBuilder.Entity<Plot>(entity =>
            {
                entity.HasKey(e => e.PlotId);
                entity.HasIndex(e => new { e.BuildingId, e.Number }).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.MarketingMessage).HasMaxLength(500);

                entity.HasOne(e => e.Building)
                      .WithMany(b => b.Plots)
                      .HasForeignKey(e => e.BuildingId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.HouseType)
                      .WithMany(ht => ht.Plots)
                      .HasForeignKey(e => e.HouseTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
             * Image
             * ========================= */
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.Property(e => e.FileName)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.FilePath)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.AltText)
                      .HasMaxLength(200);

                entity.Property(e => e.OwnerType)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(e => e.OwnerId)
                      .IsRequired();

                entity.Property(e => e.UploadedAt)
                      .HasDefaultValueSql("datetime('now')");

                entity.HasIndex(e => new { e.OwnerType, e.OwnerId });
            });

            SeedData(modelBuilder);
        }

        /* =========================
         * Seed Data
         * ========================= */
        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Development>().HasData(
                new Development
                {
                    DevelopmentId = 1,
                    Name = "Green Park Residence",
                    Location = "București, Sector 2",
                    CreatedAt = new DateTime(2024, 1, 1)
                });

            modelBuilder.Entity<Phase>().HasData(
                new Phase
                {
                    PhaseId = 1,
                    DevelopmentId = 1,
                    Name = "Faza 1",
                    StartDate = new DateTime(2024, 3, 1)
                });

            modelBuilder.Entity<Building>().HasData(
                new Building
                {
                    BuildingId = 1,
                    PhaseId = 1,
                    Name = "Blocul A"
                });

            modelBuilder.Entity<HouseType>().HasData(
                new HouseType
                {
                    HouseTypeId = 1,
                    PhaseId = 1,
                    Name = "Tip A - 2 camere",
                    Bedrooms = 2,
                    Bathrooms = 1,
                    FromPrice = 85000,
                    PropertyType = PropertyType.Apartment,
                    IsAvailable = true,
                    IsFeatured = true
                });

            modelBuilder.Entity<Plot>().HasData(
                new Plot
                {
                    PlotId = 1,
                    BuildingId = 1,
                    HouseTypeId = 1,
                    Number = 101,
                    Name = "Apartament 101",
                    Price = 87500,
                    Status = PlotStatus.Available,
                    Level = 1,
                    IsFeatured = true
                });
        }
    }
}
