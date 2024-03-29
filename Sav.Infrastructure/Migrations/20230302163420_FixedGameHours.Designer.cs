﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sav.Infrastructure;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    [DbContext(typeof(SteamContext))]
    [Migration("20230302163420_FixedGameHours")]
    partial class FixedGameHours
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("Sav.Infrastructure.Entities.AchievementEntity", b =>
                {
                    b.Property<string>("AppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Apiname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IconClosed")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IconOpen")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("Percent")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("AppID", "Apiname");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.GameEntity", b =>
                {
                    b.Property<string>("AppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("GameIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("GameLogoSmall")
                        .HasColumnType("TEXT");

                    b.Property<string>("GlobalStatsLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("HoursLast2Weeks")
                        .HasColumnType("TEXT");

                    b.Property<string>("HoursOnRecord")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StoreLink")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("AppID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserAchievementEntity", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Apiname")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UnlockTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "AppID", "Apiname");

                    b.HasIndex("AppID", "Apiname");

                    b.ToTable("UserAchievements");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserEntity", b =>
                {
                    b.Property<string>("SteamID64")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarFull")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarIcon")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarMedium")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomURL")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("HoursPlayed2Wk")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<int>("IsLimitedAccount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("TEXT");

                    b.Property<string>("OnlineState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PrivacyState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Realname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StateMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SteamID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TradeBanState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<int>("VacBanned")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VisibilityState")
                        .HasColumnType("INTEGER");

                    b.HasKey("SteamID64");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserGameEntity", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("HoursLast2Weeks")
                        .HasColumnType("TEXT");

                    b.Property<string>("HoursOnRecord")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<string>("StatsLink")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "AppID");

                    b.HasIndex("AppID");

                    b.ToTable("UserGames");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.AchievementEntity", b =>
                {
                    b.HasOne("Sav.Infrastructure.Entities.GameEntity", "Game")
                        .WithMany("Achievements")
                        .HasForeignKey("AppID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserAchievementEntity", b =>
                {
                    b.HasOne("Sav.Infrastructure.Entities.GameEntity", null)
                        .WithMany("UserAchievements")
                        .HasForeignKey("AppID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sav.Infrastructure.Entities.UserEntity", "User")
                        .WithMany("UserAchievements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sav.Infrastructure.Entities.AchievementEntity", "Achievement")
                        .WithMany("UserAchievements")
                        .HasForeignKey("AppID", "Apiname")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Achievement");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserGameEntity", b =>
                {
                    b.HasOne("Sav.Infrastructure.Entities.GameEntity", "Game")
                        .WithMany("UserGames")
                        .HasForeignKey("AppID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sav.Infrastructure.Entities.UserEntity", "User")
                        .WithMany("UserGames")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.AchievementEntity", b =>
                {
                    b.Navigation("UserAchievements");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.GameEntity", b =>
                {
                    b.Navigation("Achievements");

                    b.Navigation("UserAchievements");

                    b.Navigation("UserGames");
                });

            modelBuilder.Entity("Sav.Infrastructure.Entities.UserEntity", b =>
                {
                    b.Navigation("UserAchievements");

                    b.Navigation("UserGames");
                });
#pragma warning restore 612, 618
        }
    }
}
