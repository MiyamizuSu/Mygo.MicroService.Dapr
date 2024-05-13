using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Contrib.MaskedTestList.Api.Models;

namespace RecAll.Contrib.MaskedTestList.Api.Services;

public class MaskedTextListContext : DbContext
{
    public DbSet<MaskedTextItem> MaskedTextItems { get; set; }
    public MaskedTextListContext(DbContextOptions<MaskedTextListContext> options) :
        base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new TextItemConfiguration());
    }
    
    public class TextItemConfiguration : IEntityTypeConfiguration<MaskedTextItem> {
        public void Configure(EntityTypeBuilder<MaskedTextItem> builder) {
            builder.ToTable("maskedtextitems");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseHiLo("maskedtextitem_hilo");

            builder.Property(p => p.ItemId).IsRequired(false);
            builder.HasIndex(p => p.ItemId).IsUnique();

            builder.Property(p => p.Content).IsRequired();

            builder.Property(p => p.UserIdentityGuid).IsRequired();
            builder.HasIndex(p => p.UserIdentityGuid).IsUnique(false);

            builder.Property(p => p.IsDeleted).IsRequired();
        }
    }
    public class TextListContextDesignFactory : IDesignTimeDbContextFactory<MaskedTextListContext> {
        public MaskedTextListContext CreateDbContext(string[] args) =>
            new(new DbContextOptionsBuilder<MaskedTextListContext>()
                .UseSqlServer(
                    "Server=.;Initial Catalog=RecAll.MaskedTextListDb;Integrated Security=true")
                .Options);
    }
}