using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TrollChat.DataAccess.Context;

namespace TrollChat.DataAccess.Migrations
{
    [DbContext(typeof(TrollChatDbContext))]
    [Migration("20170322124215_initial2")]
    partial class initial2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TrollChat.DataAccess.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<DateTime?>("EmailConfirmedOn");

                    b.Property<DateTime?>("LockedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(128)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
        }
    }
}
