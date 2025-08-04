using System.ComponentModel.DataAnnotations;

namespace TeacherService.Model
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Subject { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
