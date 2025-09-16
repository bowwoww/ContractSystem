using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class AppDbContext : DbContext
    {
        //依賴注入
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberRole> MemberRoles { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<ContractEdit> ContractEdits { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<TrainingClass> TrainingClasses {  get; set; }

        public DbSet<TrainingDate> TrainingDates { get; set; }

        public DbSet<PayType> PayTypes { get; set; }

        public DbSet<KnowSource> KnowSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        //    //設定要建立的資料表和欄位的對應關係
        modelBuilder.Entity<Member>(entiry => 
            {
                entiry.ToTable("Members");
                entiry.HasKey(m => m.MemberID);
                entiry.Property(m => m.MemberName).IsRequired().HasMaxLength(50);
                entiry.Property(m => m.MemberPassword).IsRequired().HasMaxLength(256);
                entiry.Property(m => m.MemberRole).IsRequired().HasMaxLength(1);
                entiry.Property(m => m.MemberTel).HasMaxLength(15);
                entiry.Property(m => m.LineID).HasMaxLength(20);
                entiry.Property(m => m.MemberBirthday);
                entiry.Property(m => m.MemberGender).IsRequired();
                entiry.Property(m => m.MemberAddress).HasMaxLength(100);
                entiry.Property(m => m.MemberRemark).HasMaxLength(1000);
                entiry.HasOne(m => m.MemberRoleNavigation)
                      .WithMany(mr => mr.Members)
                      .HasForeignKey(m => m.MemberRole)
                      .OnDelete(DeleteBehavior.Restrict);
                entiry.HasOne(m => m.KnowSource)
                        .WithMany(ks => ks.Members)
                        .HasForeignKey(m => m.MemberSource)
                        .OnDelete(DeleteBehavior.SetNull);
                entiry.HasMany(m => m.Contracts)
                      .WithOne(c => c.Member)
                      .HasForeignKey(c => c.MemberID)
                      .OnDelete(DeleteBehavior.NoAction);
                entiry.HasMany(m => m.TrainingDates)
                      .WithOne(td => td.Member)
                      .HasForeignKey(td => td.TrainerID)
                      .OnDelete(DeleteBehavior.NoAction);


            });
            modelBuilder.Entity<MemberRole>(entity =>
            {
                entity.ToTable("MemberRoles");
                entity.HasKey(mr => mr.MemberRoleID);
                entity.Property(mr => mr.MemberRoleName).IsRequired().HasMaxLength(20);
                entity.HasMany(mr => mr.Members)
                      .WithOne(m => m.MemberRoleNavigation)
                      .HasForeignKey(m => m.MemberRole)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<MemberRole>(entity => {
                entity.HasData(
                    new MemberRole { MemberRoleID = "A", MemberRoleName = "會員" },
                    new MemberRole { MemberRoleID = "B", MemberRoleName = "教練" },
                    new MemberRole { MemberRoleID = "C", MemberRoleName = "後台" }
                );
            });
            //設定合約資料表從 Contract.cs
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contracts");
                entity.HasKey(c => c.ContractID);
                entity.Property(c => c.ContractID).IsRequired().HasMaxLength(9);
                entity.Property(c => c.Signer).IsRequired().HasMaxLength(50);
                entity.Property(c => c.MemberID).IsRequired().HasMaxLength(6);
                entity.Property(c => c.TrainerID).IsRequired().HasMaxLength(6);
                entity.Property(c => c.HandlerID).IsRequired().HasMaxLength(6);
                entity.Property(c => c.SignDate).IsRequired();
                entity.Property(c => c.EndDate).IsRequired();
                entity.Property(c => c.ClassTypeID).IsRequired().HasMaxLength(3);
                entity.Property(c => c.PayTypeID).HasMaxLength(1);

                entity.HasOne(c => c.Member)
                      .WithMany(m => m.Contracts)
                      .HasForeignKey(c => c.MemberID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Trainer)
                      .WithMany()
                      .HasForeignKey(c => c.TrainerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Handler)
                      .WithMany()
                      .HasForeignKey(c => c.HandlerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TrainingClass)
                      .WithMany()
                      .HasForeignKey(c => c.ClassTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.PayType)
                      .WithMany()
                      .HasForeignKey(c => c.PayTypeID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.TrainingDates)
                      .WithOne(td => td.Contract)
                      .HasForeignKey(td => td.TrainingDateID)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(c => c.Attendances)
                      .WithOne(a => a.Contract)
                      .HasForeignKey(a => a.AttendanceID)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(c => c.ContractEdits)
                      .WithOne(ce => ce.Contract)
                      .HasForeignKey(ce => ce.ContractID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            //從ContractEdit.cs套用資料
            modelBuilder.Entity<ContractEdit>(entity =>
            {
                entity.ToTable("ContractEdits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ContractID).IsRequired().HasMaxLength(9);
                entity.Property(e => e.HandlerID).IsRequired().HasMaxLength(6);
                entity.Property(e => e.EditDate).IsRequired();
                entity.Property(e => e.EditType).IsRequired();
                entity.Property(e => e.NewEndDate);
                entity.Property(e => e.AddClassCount);
                entity.Property(e => e.TransferToMemberID).HasMaxLength(6);
                entity.Property(e => e.TransferClassCount);
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(e => e.Contract)
                        .WithMany(c => c.ContractEdits)
                        .HasForeignKey(e => e.ContractID)
                        .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Handler)
                        .WithMany()
                        .HasForeignKey(e => e.HandlerID)
                        .OnDelete(DeleteBehavior.Restrict);
            });

            //從Attendance.cs套用資料
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("Attendances");
                entity.HasKey(a => a.AttendanceID);
                entity.Property(a => a.AttendanceID).IsRequired().HasMaxLength(36);
                entity.Property(a => a.ContractID).IsRequired().HasMaxLength(9);
                entity.Property(a => a.MemberID).IsRequired().HasMaxLength(6);
                entity.Property(a => a.TrainerID).IsRequired().HasMaxLength(6);
                entity.Property(a => a.AttendanceDate).IsRequired();

                entity.HasOne(a => a.Contract)
                      .WithMany(c => c.Attendances)
                      .HasForeignKey(a => a.ContractID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Member)
                      .WithMany()
                      .HasForeignKey(a => a.MemberID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Trainer)
                      .WithMany()
                      .HasForeignKey(a => a.TrainerID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<KnowSource>(entity =>
            {
                entity.ToTable("KnowSources");
                entity.HasKey(ks => ks.Id);
                entity.Property(ks => ks.Id).ValueGeneratedOnAdd();
                entity.Property(ks => ks.SourceName).IsRequired().HasMaxLength(20);
                entity.HasMany(ks => ks.Members)
                      .WithOne(m => m.KnowSource)
                      .HasForeignKey(m => m.MemberSource)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            //從PayType.cs套用資料
            modelBuilder.Entity<PayType>(entity =>
            {
                entity.ToTable("PayTypes");
                entity.HasKey(pt => pt.PayTypeID);
                entity.Property(pt => pt.PayTypeID).IsRequired().HasMaxLength(1);
                entity.Property(pt => pt.PayTypeName).IsRequired().HasMaxLength(20);
                entity.HasMany(pt => pt.Contracts)
                      .WithOne(c => c.PayType)
                      .HasForeignKey(c => c.PayTypeID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            //從TrainingClass.cs套用資料
            modelBuilder.Entity<TrainingClass>(entity =>
            {
                entity.ToTable("TrainingClasses");
                entity.HasKey(tc => tc.ClassTypeID);
                entity.Property(tc => tc.ClassTypeID).IsRequired().HasMaxLength(3);
                entity.Property(tc => tc.ClassName).IsRequired().HasMaxLength(50);
                entity.Property(tc => tc.ClassLength).IsRequired();
                entity.Property(tc => tc.ClassDescription).HasMaxLength(200);
                entity.HasMany(tc => tc.Contracts)
                      .WithOne(c => c.TrainingClass)
                      .HasForeignKey(c => c.ClassTypeID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //從TrainingDate.cs套用資料
            modelBuilder.Entity<TrainingDate>(entity =>
            {
                entity.ToTable("TrainingDates");
                entity.HasKey(td => td.TrainingDateID);
                entity.Property(td => td.TrainingDateID)
                      .IsRequired()
                      .HasMaxLength(12);
                entity.Property(td => td.ClassDate)
                      .IsRequired()
                      .HasColumnType("datetime");
                entity.Property(td => td.TrainerID)
                      .IsRequired()
                      .HasMaxLength(6);

                // 訓練日期與合約的關聯
                entity.HasOne(c => c.Contract)
                      .WithMany(c => c.TrainingDates)
                      .HasForeignKey(td => td.ContractID)
                      .OnDelete(DeleteBehavior.Cascade);

                // 訓練日期與會員的關聯
                entity.HasOne(td => td.Member)
                      .WithMany(m => m.TrainingDates)
                      .HasForeignKey(td => td.TrainerID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            //從Store.cs套用資料
            
        }
    }
}
