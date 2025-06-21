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
        => optionsBuilder.UseSqlServer("server=GAMING;User Id=sa;Password=Pa55w.rd;Database=RWA;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC071AE2B53D");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC07180E5E08");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC07F569F216");

            entity.Property(e => e.Approved).HasDefaultValue(false);
            entity.Property(e => e.PostedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts).HasConstraintName("FK__Post__TopicId__49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.Posts).HasConstraintName("FK__Post__UserId__48CFD27E");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rating__3214EC07B9127BF1");

            entity.Property(e => e.RatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Post).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__PostId__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__UserId__52593CB8");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tag__3214EC07E68DDF29");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topic__3214EC07B3CE6646");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Topics).HasConstraintName("FK__Topic__CategoryI__3D5E1FD2");

            entity.HasMany(d => d.Tags).WithMany(p => p.Topics)
                .UsingEntity<Dictionary<string, object>>(
                    "TopicTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__TopicTag__TagId__440B1D61"),
                    l => l.HasOne<Topic>().WithMany()
                        .HasForeignKey("TopicId")
                        .HasConstraintName("FK__TopicTag__TopicI__4316F928"),
                    j =>
                    {
                        j.HasKey("TopicId", "TagId").HasName("PK__TopicTag__D479C0C710C26AFF");
                        j.ToTable("TopicTag");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0782266027");

            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
