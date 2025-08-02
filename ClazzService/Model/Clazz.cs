namespace ClazzService.Model
{
    public class Clazz
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!; // e.g., "Grade 1"

        public int? TeacherId { get; set; } // assigned teacher

        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
    }
}
