namespace StudentService.DTO
{
    public class StudentDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? ClazzId { get; set; }
    }
}

