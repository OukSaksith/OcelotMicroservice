using Microsoft.EntityFrameworkCore;
using StudentService.DTO;
using StudentService.Model;
using StudentService.Service;
using System.Reflection;

    namespace StudentService.Services;

    public class StudentServiceImpl : IStudentService
    {
        private readonly StudentDbContext _db;

        public StudentServiceImpl(StudentDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _db.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    ClazzId = s.ClazzId,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    EnrollmentDate = s.EnrollmentDate ?? DateTime.UtcNow

                })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return null;
            return new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                ClazzId = s.ClazzId,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address,
                DateOfBirth = s.DateOfBirth,
                EnrollmentDate = s.EnrollmentDate ?? DateTime.UtcNow
            };
        }

        public async Task<StudentDto> CreateAsync(StudentDto dto)
        {
            var entity = new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                ClazzId = dto.ClazzId,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                EnrollmentDate = dto.EnrollmentDate ?? DateTime.UtcNow
            };
            _db.Students.Add(entity);
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, StudentDto dto)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return false;
            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.ClazzId = dto.ClazzId;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Address = dto.Address;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.EnrollmentDate = dto.EnrollmentDate ?? DateTime.UtcNow;
        await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return false;
            _db.Students.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
