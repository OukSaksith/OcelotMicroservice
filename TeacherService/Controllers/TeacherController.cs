using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeacherService.DTO;
using TeacherService.Services;

namespace TeacherService.Controllers
{
    [ApiController]
    [Route("api/v1/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _svc;
        public TeachersController(ITeacherService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherDto>> Get(int id)
        {
            var t = await _svc.GetByIdAsync(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> Create(TeacherDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, TeacherDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            if (!await _svc.UpdateAsync(id, dto)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _svc.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
