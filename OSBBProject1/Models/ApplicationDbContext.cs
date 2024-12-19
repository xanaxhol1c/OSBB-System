using Microsoft.EntityFrameworkCore;
using OSBBProject1.Models;

namespace OSBBProject1
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<ChangeHistory> ChangeHistories { get; set; }

        public DbSet<Bill> Bills { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Login).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Password).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Flat>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            modelBuilder.Entity<HelpRequest>(entity =>
            {
                entity.HasKey(h => h.Id); // Визначення первинного ключа
                entity.Property(h => h.UserName).IsRequired().HasMaxLength(100); // Ім'я користувача обов'язкове
                entity.Property(h => h.Text).IsRequired(); // Текст скарги обов'язковий
                entity.Property(h => h.SendDate).IsRequired(); // Дата відправлення обов'язкова
                entity.Property(h => h.Status).IsRequired(); // Статус (не прочитана/прочитана) обов'язковий
            });

            modelBuilder.Entity<Resident>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FlatNumber).IsRequired();
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Login).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Password).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<ChangeHistory>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Action).IsRequired().HasMaxLength(100);
                entity.Property(c => c.ChangedBy).IsRequired().HasMaxLength(100);
                entity.Property(c => c.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(c => c.OldValue).HasMaxLength(500);
                entity.Property(c => c.NewValue).HasMaxLength(500);
                entity.Property(c => c.ChangeDate).IsRequired();
            });


            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.UserId);
                entity.Property(c => c.LightBill);
                entity.Property(c => c.WaterBill);
                entity.Property(c => c.Status);
            });
        }
    }
}
