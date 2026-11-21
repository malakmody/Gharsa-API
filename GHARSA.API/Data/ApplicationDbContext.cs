using GHARSA.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GHARSA.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DATABASE TABLES
        // =========================

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Plant> Plants { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<MyPlant> MyPlants { get; set; } = null!;
        public DbSet<PlantCareGuide> PlantCareGuides { get; set; } = null!;
        public DbSet<CareTask> CareTasks { get; set; } = null!;
        public DbSet<PlantCareTask> PlantCareTasks { get; set; } = null!;
        public DbSet<CareLog> CareLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // CATEGORY -> PLANTS
            // ONE-TO-MANY
            // =========================

            modelBuilder.Entity<Plant>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Plants)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // USER -> MY PLANTS
            // ONE-TO-MANY
            // =========================

            modelBuilder.Entity<MyPlant>()
                .HasOne(mp => mp.User)
                .WithMany(u => u.MyPlants)
                .HasForeignKey(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // PLANT -> MY PLANTS
            // ONE-TO-MANY
            // =========================

            modelBuilder.Entity<MyPlant>()
                .HasOne(mp => mp.Plant)
                .WithMany(p => p.MyPlants)
                .HasForeignKey(mp => mp.PlantId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // MY PLANT -> CARE LOGS
            // ONE-TO-MANY
            // =========================

            modelBuilder.Entity<CareLog>()
                .HasOne(cl => cl.MyPlant)
                .WithMany(mp => mp.CareLogs)
                .HasForeignKey(cl => cl.MyPlantId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // PLANT -> PLANT CARE GUIDE
            // ONE-TO-ONE
            // =========================

            modelBuilder.Entity<PlantCareGuide>()
                .HasOne(g => g.Plant)
                .WithOne(p => p.PlantCareGuide)
                .HasForeignKey<PlantCareGuide>(
                    g => g.PlantId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // PLANT <-> CARE TASK
            // MANY-TO-MANY
            // =========================

            modelBuilder.Entity<PlantCareTask>()
                .HasKey(pct => new
                {
                    pct.PlantId,
                    pct.CareTaskId
                });

            modelBuilder.Entity<PlantCareTask>()
                .HasOne(pct => pct.Plant)
                .WithMany(p => p.PlantCareTasks)
                .HasForeignKey(pct => pct.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlantCareTask>()
                .HasOne(pct => pct.CareTask)
                .WithMany(ct => ct.PlantCareTasks)
                .HasForeignKey(pct => pct.CareTaskId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // UNIQUE USER EMAIL
            // =========================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // =========================
            // PREVENT DUPLICATE USER PLANTS
            // =========================

            modelBuilder.Entity<MyPlant>()
                .HasIndex(mp => new
                {
                    mp.UserId,
                    mp.PlantId
                })
                .IsUnique();


            // =========================
            // PLANT CARE GUIDE DEFAULT VALUE
            // =========================

            modelBuilder.Entity<PlantCareGuide>()
                .Property(g => g.WateringIntervalDays)
                .HasDefaultValue(1);


            // =========================
            // WATERING INTERVAL VALIDATION
            // =========================

            modelBuilder.Entity<PlantCareGuide>()
    .HasCheckConstraint(
        "CK_PlantCareGuides_WateringIntervalDays_Positive",
        "[WateringIntervalDays] > 0"
    );
        }
    }
}