using Microsoft.EntityFrameworkCore;
using TeacherService.Model;

namespace TeacherService
{
    public class TeacherDbContext : DbContext
    {
        public TeacherDbContext(DbContextOptions<TeacherDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers => Set<Teacher>();
    }

}
