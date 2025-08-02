using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentService.DTO;
using StudentService.Service;


namespace StudentService.Controllers;

[ApiController]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _svc;

    public StudentsController(IStudentService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> Get(int id)
    {
        var s = await _svc.GetByIdAsync(id);
        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(StudentDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StudentDto dto)
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