using Microsoft.EntityFrameworkCore;
using StudentService.Model;

namespace StudentService
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
    }
}
