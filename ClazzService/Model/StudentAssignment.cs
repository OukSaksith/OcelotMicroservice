namespace ClazzService.Model
{
    public class StudentAssignment
    {
        public int Id { get; set; }
        public int ClazzId { get; set; }
        public Clazz Clazz { get; set; } = null!;
        public int StudentId { get; set; }
    }
}
