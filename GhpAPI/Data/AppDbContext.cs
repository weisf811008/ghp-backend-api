using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<School> Schools { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CategoryItem> Categories { get; set; }
        public DbSet<InspectionItem> Items { get; set; }
        public DbSet<Regulation> Regulations { get; set; }
        public DbSet<VisitingForm> VisitingForms { get; set; }
        public DbSet<ItemRegulation> ItemRegulations { get; set; }
        public DbSet<ItemVisitingForm> ItemVisitingForms { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormDetail> FormDetails { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionDetail> InspectionDetails { get; set; }
        public DbSet<InspectionFile> InspectionFiles { get; set; }
        public DbSet<History> Histories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 統一設定有 CreatedAt/UpdatedAt 的 Entity 預設值
            var entityTypes = new[]
            {
                typeof(School), typeof(User), typeof(Role),
                typeof(CategoryItem), typeof(InspectionItem),
                typeof(Regulation), typeof(VisitingForm), typeof(Form)
            };

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("getdate()");

                modelBuilder.Entity(entityType)
                    .Property("UpdatedAt")
                    .HasDefaultValueSql("getdate()");
            }

            modelBuilder.Entity<Inspection>()
                .Property(i => i.CreatedAt)
                .HasDefaultValueSql("getdate()");

            // 明確設定關聯表的主鍵為自動遞增
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id);
            modelBuilder.Entity<UserRole>()
                .Property(ur => ur.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ItemRegulation>()
                .HasKey(ir => ir.Id);
            modelBuilder.Entity<ItemRegulation>()
                .Property(ir => ir.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ItemVisitingForm>()
                .HasKey(iv => iv.Id);
            modelBuilder.Entity<ItemVisitingForm>()
                .Property(iv => iv.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FormDetail>()
                .HasKey(fd => fd.Id);
            modelBuilder.Entity<FormDetail>()
                .Property(fd => fd.Id)
                .ValueGeneratedOnAdd();

            // 關聯設定
            modelBuilder.Entity<User>()
                .HasOne(u => u.School)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SchoolId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<InspectionItem>()
                .HasOne(i => i.CategoryItem)
                .WithMany()
                .HasForeignKey(i => i.CategoryId);

            modelBuilder.Entity<ItemRegulation>()
                .HasOne(ir => ir.Item)
                .WithMany(i => i.ItemRegulations)
                .HasForeignKey(ir => ir.ItemId);

            modelBuilder.Entity<ItemRegulation>()
                .HasOne(ir => ir.Regulation)
                .WithMany(r => r.ItemRegulations)
                .HasForeignKey(ir => ir.RegulationId);

            modelBuilder.Entity<ItemVisitingForm>()
                .HasOne(iv => iv.Item)
                .WithMany(i => i.ItemVisitingForms)
                .HasForeignKey(iv => iv.ItemId);

            modelBuilder.Entity<ItemVisitingForm>()
                .HasOne(iv => iv.VisitingForm)
                .WithMany(v => v.ItemVisitingForms)
                .HasForeignKey(iv => iv.VisitingFormId);

            modelBuilder.Entity<InspectionDetail>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<InspectionDetail>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<InspectionFile>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<InspectionFile>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<History>()
                .Property(h => h.Timestamp)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<History>()
                .HasKey(h => h.Id);
            modelBuilder.Entity<History>()
                .Property(h => h.Id)
                .ValueGeneratedOnAdd();

            // 指定資料表名稱
            modelBuilder.Entity<CategoryItem>().ToTable("Categories");
            modelBuilder.Entity<InspectionItem>().ToTable("Items");
        }
    }
}