using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            modelBuilder.Entity<Mentor>().ToTable("Mentors");
            modelBuilder.Entity<Company>().OwnsOne(c => c.Address);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chair> Chairs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student>  Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DegreeProgram> DegreePrograms { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<Holiday> Holidays {  get; set; }
    }
}
