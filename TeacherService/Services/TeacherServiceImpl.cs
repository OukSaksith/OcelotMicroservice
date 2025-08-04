using Microsoft.EntityFrameworkCore;
using TeacherService.DTO;
using TeacherService.Model;

namespace TeacherService.Services
{
    public class TeacherServiceImpl : ITeacherService
    {
        private readonly TeacherDbContext _db;
        public TeacherServiceImpl(TeacherDbContext db) => _db = db;

        public async Task<IEnumerable<TeacherDto>> GetAllAsync()
            => await _db.Teachers
                .Select(t => new TeacherDto { Id = t.Id, Name = t.Name, Subject = t.Subject , Email = t.Email, Address = t.Address, PhoneNumber = t.PhoneNumber})
                .ToListAsync();

        public async Task<TeacherDto?> GetByIdAsync(int id)
        {
            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return null;
            return new TeacherDto { Id = t.Id, Name = t.Name, Subject = t.Subject, Email = t.Email, Address = t.Address, PhoneNumber = t.PhoneNumber };
        }

        public async Task<TeacherDto> CreateAsync(TeacherDto dto)
        {
            var entity = new Teacher { Name = dto.Name, Subject = dto.Subject, Email = dto.Email, Address = dto.Address, PhoneNumber = dto.PhoneNumber };
            _db.Teachers.Add(entity);
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, TeacherDto dto)
        {
            var existing = await _db.Teachers.FindAsync(id);
            if (existing == null) return false;
            existing.Name = dto.Name;
            existing.Subject = dto.Subject;
            existing.Email = dto.Email;
            existing.Address = dto.Address;
            existing.PhoneNumber = dto.PhoneNumber;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Teachers.FindAsync(id);
            if (existing == null) return false;
            _db.Teachers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
