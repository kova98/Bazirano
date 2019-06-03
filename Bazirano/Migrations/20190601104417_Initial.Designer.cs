﻿// <auto-generated />
using System;
using Bazirano.Models.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bazirano.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190601104417_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bazirano.Models.BoardPost", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("BoardThreadId");

                    b.Property<DateTime>("DatePosted");

                    b.Property<string>("Image");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("BoardThreadId");

                    b.ToTable("BoardPosts");
                });

            modelBuilder.Entity("Bazirano.Models.BoardThread", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ImageCount");

                    b.Property<bool>("IsLocked");

                    b.Property<int>("PostCount");

                    b.HasKey("Id");

                    b.ToTable("BoardThreads");
                });

            modelBuilder.Entity("Bazirano.Models.BoardPost", b =>
                {
                    b.HasOne("Bazirano.Models.BoardThread")
                        .WithMany("Posts")
                        .HasForeignKey("BoardThreadId");
                });
#pragma warning restore 612, 618
        }
    }
}