using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TimeTortoise.DAL;

namespace TimeTortoise.DAL.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20170314043939_TimeSegments")]
    partial class TimeSegments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("TimeTortoise.Model.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("TimeTortoise.Model.TimeSegment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityId");

                    b.Property<DateTime>("EndTime");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.ToTable("TimeSegments");
                });

            modelBuilder.Entity("TimeTortoise.Model.TimeSegment", b =>
                {
                    b.HasOne("TimeTortoise.Model.Activity")
                        .WithMany("TimeSegments")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
