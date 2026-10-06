using Microsoft.EntityFrameworkCore;

namespace BackEnd.BusinessObjects;

public partial class FUNewsManagementDbContext : DbContext
{
    public FUNewsManagementDbContext()
    {
    }

    public FUNewsManagementDbContext(DbContextOptions<FUNewsManagementDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<NewsArticle> NewsArticles { get; set; }
    public virtual DbSet<SystemAccount> SystemAccounts { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }

    public const string ConnectionStringName = "MyCnn";

    // Used when a DAO creates the context with "new": the connection string "MyCnn" is read from appsettings.json
    private static string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        return config.GetConnectionString(ConnectionStringName)
               ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found.");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CategoryDesciption).HasMaxLength(250);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryID)
                .HasConstraintName("FK_Category_Category");
        });

        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.ToTable("NewsArticle");
            entity.HasKey(e => e.NewsArticleID);
            entity.Property(e => e.NewsArticleID).HasMaxLength(20);
            entity.Property(e => e.NewsTitle).HasMaxLength(400);
            entity.Property(e => e.Headline).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NewsContent).HasMaxLength(4000);
            entity.Property(e => e.NewsSource).HasMaxLength(400);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.NewsArticles)
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NewsArticle_Category");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.NewsArticles)
                .HasForeignKey(d => d.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NewsArticle_SystemAccount");

            entity.HasMany(d => d.Tags).WithMany(p => p.NewsArticles)
                .UsingEntity<Dictionary<string, object>>(
                    "NewsTag",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NewsTag_Tag"),
                    l => l.HasOne<NewsArticle>().WithMany().HasForeignKey("NewsArticleID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NewsTag_NewsArticle"),
                    j =>
                    {
                        j.HasKey("NewsArticleID", "TagID");
                        j.ToTable("NewsTag");
                        j.IndexerProperty<string>("NewsArticleID").HasMaxLength(20);
                    });
        });

        modelBuilder.Entity<SystemAccount>(entity =>
        {
            entity.ToTable("SystemAccount");
            entity.HasKey(e => e.AccountID);
            entity.Property(e => e.AccountID).ValueGeneratedNever();
            entity.Property(e => e.AccountName).HasMaxLength(100);
            entity.Property(e => e.AccountEmail).HasMaxLength(70);
            entity.Property(e => e.AccountPassword).HasMaxLength(70);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(e => e.TagID).HasName("PK_HashTag");
            entity.Property(e => e.TagID).ValueGeneratedNever();
            entity.Property(e => e.TagName).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(400);
        });
    }
}
