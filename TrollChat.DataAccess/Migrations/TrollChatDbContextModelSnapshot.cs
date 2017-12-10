using System;
using BriskChat.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BriskChat.DataAccess.Migrations
{
    [DbContext(typeof(TrollChatDbContext))]
    partial class TrollChatDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TrollChat.DataAccess.Models.Domain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<Guid?>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.EmailMessage", b =>
                {
                    b.Property<Guid>("Id")
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

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("EmailMessages");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<Guid>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("UserRoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("Customization");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<Guid>("DomainId");

                    b.Property<bool>("IsPrivateConversation");

                    b.Property<bool>("IsPublic");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<Guid>("OwnerId");

                    b.Property<string>("Topic")
                        .HasColumnType("NVARCHAR(100)");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.RoomTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<Guid>("RoomId");

                    b.Property<Guid>("TagId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("TagId");

                    b.ToTable("RoomTags");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<Guid?>("RoomId");

                    b.Property<Guid?>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserRoomId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<Guid>("DomainId");

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

                    b.HasIndex("DomainId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserDomain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<Guid>("DomainId");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<Guid>("RoleId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserDomains");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<Guid?>("LastMessageId");

                    b.Property<DateTime?>("LockedUntil");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<Guid?>("RoleId");

                    b.Property<Guid>("RoomId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("LastMessageId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRooms");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoomTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<Guid>("TagId");

                    b.Property<Guid>("UserRoomId");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.HasIndex("UserRoomId");

                    b.ToTable("UserRoomTags");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("SecretToken")
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<DateTime?>("SecretTokenTimeStamp")
                        .IsRequired();

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Domain", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Message", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.UserRoom", "UserRoom")
                        .WithMany("Messages")
                        .HasForeignKey("UserRoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.Room", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Domain", "Domain")
                        .WithMany("Rooms")
                        .HasForeignKey("DomainId");

                    b.HasOne("TrollChat.DataAccess.Models.User", "Owner")
                        .WithMany("Rooms")
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

            modelBuilder.Entity("TrollChat.DataAccess.Models.User", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Domain", "Domain")
                        .WithMany("Users")
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserDomain", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("TrollChat.DataAccess.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("TrollChat.DataAccess.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TrollChat.DataAccess.Models.UserRoom", b =>
                {
                    b.HasOne("TrollChat.DataAccess.Models.Message", "LastMessage")
                        .WithMany()
                        .HasForeignKey("LastMessageId");

                    b.HasOne("TrollChat.DataAccess.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("TrollChat.DataAccess.Models.Room", "Room")
                        .WithMany("UserRooms")
                        .HasForeignKey("RoomId");

                    b.HasOne("TrollChat.DataAccess.Models.User", "User")
                        .WithMany("UserRooms")
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
