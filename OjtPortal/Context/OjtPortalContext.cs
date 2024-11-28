using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Entities;

namespace OjtPortal.Context
{
    public class OjtPortalContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public OjtPortalContext(DbContextOptions<OjtPortalContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<Chair>().ToTable("Chairs");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<Student>().ToTable("Students").OwnsOne(s => s.Shift);
            modelBuilder.Entity<Mentor>()
                .HasMany(m => m.SubMentors)
                .WithOne(sm => sm.HeadMentor)
                .HasForeignKey(sm => sm.HeadMentorId);
            modelBuilder.Entity<Company>().OwnsOne(c => c.Address);
            modelBuilder.Entity<TrainingTask>()
               .HasMany(t => t.TechStacks) 
               .WithMany(t => t.Tasks) 
               .UsingEntity(j => j.ToTable("TaskStack"));
            modelBuilder.Entity<SubMentor>()
               .HasMany(sm => sm.TrainingTask)
               .WithMany(t => t.SubMentor)
               .UsingEntity(j => j.ToTable("SubmentorTasks"));
            modelBuilder.Entity<TrainingTask>()
               .HasMany(t => t.Skills)
               .WithMany(t => t.Tasks)
               .UsingEntity(j => j.ToTable("TaskSkill"));
            modelBuilder.Entity<TechStack>()
                .HasIndex(t => t.Name);
            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Name);
            modelBuilder.Entity<SubMentor>().ToTable("SubMentors");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chair> Chairs { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student>  Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DegreeProgram> DegreePrograms { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<Holiday> Holidays {  get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LogbookEntry> LogbookEntries { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<TrainingTask> TrainingTasks { get; set; }
        public DbSet<TechStack> TechStacks { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<StudentTask> StudentTasks { get; set; }
        public DbSet<StudentTraining> StudentTrainings { get; set; }
        public DbSet<SubMentor> SubMentors { get; set; }
    }
}
