using DoctorPatientApp.API.Models;
using DoctorPatientApp.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Soft Delete Filters ───────────────────────────────────────
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Admin>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Doctor>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Patient>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<TimeSlot>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<MedicalRecord>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Prescription>().HasQueryFilter(x => !x.IsDeleted);

            // ── User ──────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
            });

            // ── Admin ─────────────────────────────────────────────────────
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId).IsUnique();
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.HasOne(a => a.User)
                      .WithOne(u => u.Admin)
                      .HasForeignKey<Admin>(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Doctor ────────────────────────────────────────────────────
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.Property(d => d.ConsultationFee).HasPrecision(10, 2);

                entity.HasOne(d => d.User)
                      .WithOne(u => u.Doctor)
                      .HasForeignKey<Doctor>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.AssignedAdmin)
                      .WithMany(a => a.ManagedDoctors)
                      .HasForeignKey(d => d.AssignedAdminId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Patient ───────────────────────────────────────────────────
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.HasOne(p => p.User)
                      .WithOne(u => u.Patient)
                      .HasForeignKey<Patient>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── TimeSlot ──────────────────────────────────────────────────
            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.HasOne(ts => ts.Doctor)
                      .WithMany(d => d.TimeSlots)
                      .HasForeignKey(ts => ts.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Appointment ───────────────────────────────────────────────
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.HasOne(a => a.TimeSlot)
                      .WithOne(ts => ts.Appointment)
                      .HasForeignKey<Appointment>(a => a.TimeSlotId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── MedicalRecord ─────────────────────────────────────────────
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.Property(mr => mr.Temperature).HasPrecision(4, 1);
                entity.Property(mr => mr.Weight).HasPrecision(5, 2);
                entity.Property(mr => mr.Height).HasPrecision(5, 2);

                entity.HasOne(mr => mr.Appointment)
                      .WithOne(a => a.MedicalRecord)
                      .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mr => mr.Patient)
                      .WithMany(p => p.MedicalRecords)
                      .HasForeignKey(mr => mr.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.Doctor)
                      .WithMany(d => d.MedicalRecords)
                      .HasForeignKey(mr => mr.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Prescription ──────────────────────────────────────────────
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasIndex(e => e.ReferenceId).IsUnique();
                entity.HasOne(p => p.Appointment)
                      .WithMany(a => a.Prescriptions)
                      .HasForeignKey(p => p.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Patient)
                      .WithMany(pt => pt.Prescriptions)
                      .HasForeignKey(p => p.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Doctor)
                      .WithMany(d => d.Prescriptions)
                      .HasForeignKey(p => p.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.MedicalRecord)
                      .WithMany(mr => mr.Prescriptions)
                      .HasForeignKey(p => p.MedicalRecordId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}