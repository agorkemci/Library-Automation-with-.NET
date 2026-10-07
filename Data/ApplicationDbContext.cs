using Library_Automation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAutomation.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);   // Identity tablolarını kurar, silme

        builder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        builder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Loan>()
            .HasOne(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        //burada BookId üzerinde bir filtered index oluşturuyoruz 
        //amaç: index vasıtasıyla rowlara daha hızlı ulaşabilmek
        //asıl gayemiz ise ReturnDate column'nun null olduğu rowları index üzerinde tutmak
        //bu sayede boştaki kitabın sadece bir tane aktif ödünç kaydı olabiliyor
        //aynı zamanda sadece null olan rowlar indexte tutulur, daha hızlı erişim sağlanır.
        builder.Entity<Loan>()
            .HasIndex(l => l.BookId)
            .IsUnique()
            .HasFilter("[ReturnDate] IS NULL");

    }
}