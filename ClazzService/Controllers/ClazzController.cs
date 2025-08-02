using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClazzService.Service;

namespace ClazzService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClazzController : ControllerBase
    {
        private readonly IClazzRepository _clazzRepository;
        public ClazzController(IClazzRepository clazzRepository) =>
            (_clazzRepository) = (clazzRepository);


        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_clazzRepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var student = _clazzRepository.Get(id);
            if(student is null) 
                return NotFound();

            return Ok(student);
        }
    }
}
