using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TrollChat.DataAccess.Context;

namespace TrollChat.DataAccess.Migrations
{
    [DbContext(typeof(TrollChatDbContext))]
    [Migration("20170410062213_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TrollChat.DataAccess.Models.Domain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<int>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasName("Email");

                    b.HasIndex("OwnerId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.DomainRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<int>("DomainId");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("RoomId");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("RoomId");

                    b.ToTable("DomainRooms");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.EmailLogger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("FailError");

                    b.Property<string>("FailErrorMessage");

                    b.Property<int>("FailureCount");

                    b.Property<string>("From");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Recipient")
                        .IsRequired();

                    b.Property<string>("Subject");

                    b.HasKey("Id");

                    b.ToTable("EmailLogs");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<int>("LastMessageForId");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<int>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("LastMessageForId")
                        .IsUnique();

                    b.HasIndex("UserRoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("Customization");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<bool>("IsPrivateConversation");

                    b.Property<bool>("IsPublic");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<int>("OwnerId");

                    b.Property<string>("Topic")
                        .HasColumnType("NVARCHAR(100)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.RoomTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("RoomId");

                    b.Property<int>("TagId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("TagId");

                    b.ToTable("RoomTags");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<int?>("RoomId");

                    b.Property<int?>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasName("Email");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserRoomId");

                    b.ToTable("Tags");
                });

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
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(128)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .HasName("Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime?>("LockedUntil");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("RoomId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRooms");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoomTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("TagId");

                    b.Property<int>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.HasIndex("UserRoomId");

                    b.ToTable("UserRoomTags");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("SecretToken")
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<DateTime?>("SecretTokenTimeStamp")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SecretToken")
                        .HasName("SecretToken");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Domain", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.DomainRoom", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("TrollChat.DataAccess.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Message", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.UserRoom", "LastMessageFor")
                        .WithOne("LastMessage")
                        .HasForeignKey("TrollChat.DataAccess.Models.Message", "LastMessageForId");

                    b.HasOne("TrollChat.DataAccess.Models.UserRoom", "UserRoom")
                        .WithMany("Messages")
                        .HasForeignKey("UserRoomId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Room", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.RoomTag", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.HasOne("TrollChat.DataAccess.Models.Tag", "Tag")
                        .WithMany("Room")
                        .HasForeignKey("TagId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Tag", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Room")
                        .WithMany("Tags")
                        .HasForeignKey("RoomId");

                    b.HasOne("TrollChat.DataAccess.Models.UserRoom")
                        .WithMany("Tags")
                        .HasForeignKey("UserRoomId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoom", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.HasOne("TrollChat.DataAccess.Models.User", "User")
                        .WithMany("Rooms")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoomTag", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Tag", "Tag")
                        .WithMany("UserRoom")
                        .HasForeignKey("TagId");

                    b.HasOne("TrollChat.DataAccess.Models.UserRoom", "UserRoom")
                        .WithMany()
                        .HasForeignKey("UserRoomId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserToken", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId");
                });
        }
    }
}
