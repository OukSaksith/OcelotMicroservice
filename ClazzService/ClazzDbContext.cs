using ClazzService.Model;
using Microsoft.EntityFrameworkCore;

namespace ClazzService
{
    public class ClazzDbContext : DbContext
    {
        public ClazzDbContext(DbContextOptions<ClazzDbContext> options) : base(options) { }

        public DbSet<Clazz> Clazzes => Set<Clazz>();
        public DbSet<StudentAssignment> StudentAssignments => Set<StudentAssignment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Clazz)
                .WithMany(c => c.StudentAssignments)
                .HasForeignKey(sa => sa.ClazzId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
