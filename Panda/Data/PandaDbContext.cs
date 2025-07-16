using Microsoft.EntityFrameworkCore;
using Panda.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Panda.Data;

public class PandaDbContext : DbContext
{
    public PandaDbContext(DbContextOptions<PandaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patient entity config
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.NhsNumber)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.Postcode)
                .IsRequired()
                .HasMaxLength(8);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ScheduledAt).IsRequired();
            entity.Property(e => e.DurationMinutes).IsRequired();
            entity.Property(e => e.Clinician).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Postcode).HasMaxLength(8);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
