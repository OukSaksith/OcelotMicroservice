namespace ClazzService.DTO
{
    public class ClazzDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? TeacherId { get; set; }
        public List<int> StudentIds { get; set; } = new();
    }
}
