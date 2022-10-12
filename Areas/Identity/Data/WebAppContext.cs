using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.Areas.Identity.Data;
using WebApp.Models;

namespace WebApp.Data;

public class WebAppContext : IdentityDbContext<WebAppUser>
{
    public WebAppContext(DbContextOptions<WebAppContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.HasDefaultSchema("Identity");
        builder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable(name: "User");
        });
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Role");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
        builder.Entity<Appointment>(entity =>
            entity.Property(a => a.Date)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>());

        builder.Entity<Appointment>(entity =>
        entity.Property(a => a.Time)
        .HasConversion<TimeOnlyConverter, TimeOnlyComparer>());

        builder.Entity<PatientProfile>()
            .HasOne(a => a.PatientAddress)
            .WithOne(b => b.PatientProfile)
            .HasForeignKey<PatientAddress>(c => c.PatientProfileId);
        builder.Entity<PatientProfile>()
            .HasOne(a => a.CreditCard)
            .WithOne(b => b.PatientProfile)
            .HasForeignKey<CreditCard>(c => c.PatientProfileId);
        builder.Entity<PatientProfile>()
            .HasOne(a => a.MedicalAid)
            .WithOne(b => b.PatientProfile)
            .HasForeignKey<MedicalAid>(c => c.PatientProfileId);
        builder.Entity<PatientProfile>()
            .HasOne(a => a.Work)
            .WithOne(b => b.PatientProfile)
            .HasForeignKey<Work>(c => c.PatientProfileId);
    }

    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
        {

        }
    }

    public class DateOnlyComparer : ValueComparer<DateOnly>
    {
        public DateOnlyComparer() : base(
            (d1, d2) => d1.DayNumber == d2.DayNumber,
            d => d.GetHashCode())
        {

        }
    }

    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
            timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan)
            )
        {

        }
    }

    public class TimeOnlyComparer : ValueComparer<TimeOnly>
    {
        public TimeOnlyComparer() : base(
            (t1, t2) => t1.Ticks == t2.Ticks,
            t => t.GetHashCode()
            )
        {

        }
    }

    public DbSet<WebApp.Models.Appointment> Appointment { get; set; }
    public DbSet<WebApp.Models.Suite> Suite { get; set; }

    public DbSet<WebApp.Models.Doctor> Doctor { get; set; }
    public DbSet<WebApp.Models.PatientProfile> PatientProfile { get; set; }
    public DbSet<PatientAddress> PatientAddress { get; set; }
    public DbSet<CreditCard> CreditCard { get; set; }
    public DbSet<MedicalAid> MedicalAid { get; set; }
    public DbSet<Work> Work { get; set; }
}
