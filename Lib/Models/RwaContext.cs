using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lib.Models;

public partial class RwaContext : DbContext
{
    public RwaContext()
    {
    }

    public RwaContext(DbContextOptions<RwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=GAMING;User Id=sa;Password=Pa55w.rd;Database=RWA;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC0789E933FE");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC07E5BA8AE3");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC07A12E0FDC");

            entity.Property(e => e.Approved).HasDefaultValue(false);
            entity.Property(e => e.PostedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts).HasConstraintName("FK__Post__TopicId__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Posts).HasConstraintName("FK__Post__UserId__5FB337D6");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rating__3214EC079591ACBA");

            entity.Property(e => e.RatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Post).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__PostId__68487DD7");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__UserId__6754599E");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tag__3214EC07E45DB7EF");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topic__3214EC0774010F5B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Topics).HasConstraintName("FK__Topic__CategoryI__5629CD9C");

            entity.HasMany(d => d.Tags).WithMany(p => p.Topics)
                .UsingEntity<Dictionary<string, object>>(
                    "TopicTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__TopicTag__TagId__5AEE82B9"),
                    l => l.HasOne<Topic>().WithMany()
                        .HasForeignKey("TopicId")
                        .HasConstraintName("FK__TopicTag__TopicI__59FA5E80"),
                    j =>
                    {
                        j.HasKey("TopicId", "TagId").HasName("PK__TopicTag__D479C0C7E8F3F9BC");
                        j.ToTable("TopicTag");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0755274704");

            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
