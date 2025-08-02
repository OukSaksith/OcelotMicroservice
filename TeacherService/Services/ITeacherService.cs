using TeacherService.DTO;

namespace TeacherService.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherDto>> GetAllAsync();
        Task<TeacherDto?> GetByIdAsync(int id);
        Task<TeacherDto> CreateAsync(TeacherDto dto);
        Task<bool> UpdateAsync(int id, TeacherDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
