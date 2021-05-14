using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace realtime.Models
{
    public partial class chatContext : DbContext
    {
        public chatContext()
        {
        }

        public chatContext(DbContextOptions<chatContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Connection> Connections { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=chat;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Connection>(entity =>
            {
                entity.ToTable("connection");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("con_id");

                entity.Property(e => e.SendId).HasColumnName("send_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Msg)
                    .HasMaxLength(250)
                    .HasColumnName("msg");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.ReceverId).HasColumnName("recever_id");

                entity.Property(e => e.Rname).HasMaxLength(50);

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
