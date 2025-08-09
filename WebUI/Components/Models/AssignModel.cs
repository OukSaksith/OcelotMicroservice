namespace WebUI.Components.Models
{
    public class AssignModel
    {
        public int? TeacherId { get; set; }
        public List<int> StudentIds { get; set; } = new();
    }
}
