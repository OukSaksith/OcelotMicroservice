using ClazzService.DTO;
using ClazzService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClazzService.Controllers
{
    [ApiController]
    [Route("api/v1/clazzes")]
    public class ClazzesController : ControllerBase
    {
        private readonly IClazzService _svc;

        public ClazzesController(IClazzService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClazzDto>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClazzDto>> Get(int id)
        {
            var c = await _svc.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<ActionResult<ClazzDto>> Create(ClazzDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ClazzDto dto)
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

        [HttpPost("{id:int}/assign")]
        public async Task<IActionResult> Assign(int id, AssignClassDto assignDto)
        {
            var ok = await _svc.AssignClassAsync(id, assignDto);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
