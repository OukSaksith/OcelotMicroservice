namespace StudentService.DTO
{
    public class StudentDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? ClazzId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}

