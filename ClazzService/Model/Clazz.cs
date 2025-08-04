using System.ComponentModel.DataAnnotations;

namespace ClazzService.Model
{
    public class Clazz
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!; // e.g., "Grade 1"

        public int? TeacherId { get; set; } // assigned teacher

        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();

        public string? Description { get; set; }

        [Range(1, 200)]
        public int? Capacity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
