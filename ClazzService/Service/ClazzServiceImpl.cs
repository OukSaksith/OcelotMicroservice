using ClazzService.DTO;
using ClazzService.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClazzService.Service
{
    public class ClazzServiceImpl : IClazzService
    {
        private readonly ClazzDbContext _db;

        public ClazzServiceImpl(ClazzDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ClazzDto>> GetAllAsync()
        {
            return await _db.Clazzes
                .Include(c => c.StudentAssignments)
                .Select(c => new ClazzDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    TeacherId = c.TeacherId,
                    StudentIds = c.StudentAssignments.Select(sa => sa.StudentId).ToList(),
                    Description = c.Description,
                    Capacity = c.Capacity,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<ClazzDto?> GetByIdAsync(int id)
        {
            var c = await _db.Clazzes
                .Include(c => c.StudentAssignments)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (c == null) return null;
            return new ClazzDto
            {
                Id = c.Id,
                Title = c.Title,
                TeacherId = c.TeacherId,
                StudentIds = c.StudentAssignments.Select(sa => sa.StudentId).ToList(),
                Description = c.Description,
                Capacity = c.Capacity,
                IsActive = c.IsActive
            };
        }

        public async Task<ClazzDto> CreateAsync(ClazzDto dto)
        {
            var entity = new Clazz
            {
                Title = dto.Title,
                TeacherId = dto.TeacherId,
                Description = dto.Description,
                Capacity = dto.Capacity,
                IsActive = dto.IsActive

            };
            if (dto.StudentIds != null)
            {
                entity.StudentAssignments = dto.StudentIds
                    .Select(sid => new StudentAssignment { StudentId = sid })
                    .ToList();
            }

            _db.Clazzes.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ClazzDto dto)
        {
            var existing = await _db.Clazzes
                .Include(c => c.StudentAssignments)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null) return false;

            existing.Title = dto.Title;
            existing.TeacherId = dto.TeacherId;
            existing.Description = dto.Description;
            existing.Capacity = dto.Capacity;
            existing.IsActive = dto.IsActive;


            // Sync student assignments
            var incoming = dto.StudentIds ?? new List<int>();
            var toRemove = existing.StudentAssignments
                .Where(sa => !incoming.Contains(sa.StudentId))
                .ToList();
            var toAdd = incoming
                .Where(sid => existing.StudentAssignments.All(sa => sa.StudentId != sid))
                .Select(sid => new StudentAssignment { StudentId = sid, ClazzId = existing.Id })
                .ToList();

            _db.StudentAssignments.RemoveRange(toRemove);
            await _db.StudentAssignments.AddRangeAsync(toAdd);

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Clazzes.FindAsync(id);
            if (existing == null) return false;
            _db.Clazzes.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignClassAsync(int clazzId, AssignClassDto assignDto)
        {
            var existing = await _db.Clazzes
                .Include(c => c.StudentAssignments)
                .FirstOrDefaultAsync(c => c.Id == clazzId);
            if (existing == null) return false;

            existing.TeacherId = assignDto.TeacherId;

            // Replace student list
            _db.StudentAssignments.RemoveRange(existing.StudentAssignments);
            existing.StudentAssignments = assignDto.StudentIds
                .Select(sid => new StudentAssignment { StudentId = sid, ClazzId = existing.Id })
                .ToList();

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
