using Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Database;

public class DocumentsServiceContext(DbContextOptions<DocumentsServiceContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Process> Processes { get; init; } = null!;
    public DbSet<DocumentType> AllowedDocumentTypes { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase("DocumentsServiceDatabase")
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Process>()
            .HasMany(p => p.Documents)
            .WithOne()
            .HasForeignKey(d => d.ProcessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProcessDocumentType>()
            .HasKey(pdt => new { pdt.ProcessId, pdt.DocumentTypeId });

        modelBuilder.Entity<ProcessDocumentType>()
            .HasOne(pdt => pdt.Process)
            .WithMany(p => p.AllowedDocumentTypes)
            .HasForeignKey(pdt => pdt.ProcessId);

        modelBuilder.Entity<ProcessDocumentType>()
            .HasOne(pdt => pdt.DocumentType)
            .WithMany()
            .HasForeignKey(pdt => pdt.DocumentTypeId);
        
        modelBuilder.Entity<DocumentType>()
            .HasKey(dt => dt.Id);
        
        modelBuilder.Entity<Document>()
            .HasKey(doc => doc.Id);

        base.OnModelCreating(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentType>().HasData(
            new DocumentType
            {
                IsRequired = true,
                MultipleAllowed = false,
                TypeName = "RequiredDocument1",
                Id = Guid.Parse("C97C3C00-58B4-4A5B-BD7E-4D8AB7232C78")
            },
            new DocumentType
            {
                IsRequired = true,
                MultipleAllowed = false,
                TypeName = "RequiredDocument2",
                Id = Guid.Parse("DC40094D-95DB-4AC2-A750-E9DF112FE4E6")
            },
            new DocumentType
            {
                IsRequired = true,
                MultipleAllowed = false,
                TypeName = "RequiredDocument3",
                Id = Guid.Parse("6C568F75-2DF1-409B-B400-ABED7250C0CD")
            },
            new DocumentType
            {
                IsRequired = false,
                MultipleAllowed = true,
                TypeName = "OptionalDocument1",
                Id = Guid.Parse("30A720E7-8854-4579-8D27-8F2689502A4A")
            }
        );
    }
}