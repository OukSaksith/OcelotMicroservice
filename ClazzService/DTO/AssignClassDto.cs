namespace ClazzService.DTO
{
    public class AssignClassDto
    {
        public int TeacherId { get; set; }
        public List<int> StudentIds { get; set; } = new();
    }
}
