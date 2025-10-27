using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book configuration
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Isbn).IsRequired().HasMaxLength(13);
                entity.HasIndex(b => b.Isbn).IsUnique();
                entity.Property(b => b.PublicationYear).IsRequired();
                entity.Property(b => b.TotalCopies).IsRequired();
                entity.Property(b => b.AvailableCopies).IsRequired();
                entity.Property(b => b.CreatedAt).IsRequired();
            });

            // Member configuration
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.LastName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(m => m.Email).IsUnique();
                entity.Property(m => m.Phone).HasMaxLength(20);
                entity.Property(m => m.RegistrationDate).IsRequired();
                entity.Property(m => m.IsActive).IsRequired();
            });

            // Loan configuration
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.LoanDate).IsRequired();
                entity.Property(l => l.DueDate).IsRequired();
                
                // Relationships
                entity.HasOne(l => l.Book)
                    .WithMany()
                    .HasForeignKey(l => l.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Member)
                    .WithMany()
                    .HasForeignKey(l => l.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}