﻿using System.Linq;
using BriskChat.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BriskChat.DataAccess.Context
{
    public class TrollChatDbContext : DbContext, ITrollChatDbContext
    {
        public TrollChatDbContext()
        {
        }

        public TrollChatDbContext(DbContextOptions<TrollChatDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<User>()
                .HasOne(s => s.Domain)
                .WithMany(s => s.Users);

            modelBuilder.Entity<Message>()
                .HasOne(s => s.UserRoom)
                .WithMany(s => s.Messages);

            modelBuilder.Entity<Room>()
                .HasOne(s => s.Owner)
                .WithMany(s => s.Rooms);
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void PerformMigration()
        {
            Database.Migrate();
        }

        #region DbSet

        public DbSet<Domain> Domains { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomTag> RoomTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<UserRoomTag> UserRoomTags { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<UserDomain> UserDomains { get; set; }

        #endregion DbSet
    }
}