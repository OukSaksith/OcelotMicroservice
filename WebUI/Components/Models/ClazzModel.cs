namespace WebUI.Components.Models
{
    public class ClazzModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? Capacity { get; set; }
        public List<int> StudentIds { get; set; } = new();
        public List<StudentModel> Students { get; set; } = new();
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
