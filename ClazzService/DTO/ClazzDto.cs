using ClazzService.Model;
using System.ComponentModel.DataAnnotations;

namespace ClazzService.DTO
{
    public class ClazzDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? TeacherId { get; set; }
        public List<int> StudentIds { get; set; } = new();

        public string? Description { get; set; }

        public int? Capacity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
