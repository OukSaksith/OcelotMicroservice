using ClazzService.DTO;

namespace ClazzService.Service
{
    public interface IClazzService
    {
        Task<IEnumerable<ClazzDto>> GetAllAsync();
        Task<ClazzDto?> GetByIdAsync(int id);
        Task<ClazzDto> CreateAsync(ClazzDto dto);
        Task<bool> UpdateAsync(int id, ClazzDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignClassAsync(int clazzId, AssignClassDto assignDto);
    }
}
