//https://localhost:7241/api/personapi/7fbe6a2e-7a64-4f9e-a7d2-02a3b7b1e6aa
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonApiController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetPerson(Guid id)
    {
        return Ok(new { Id = id, Name = "Ali" });
    }
}
