using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrainerController : ControllerBase
{
    private readonly TrainerMapper _mapper;
    
    public TrainerController(TrainerMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<TrainerResponse?>> GetAll()
    {
        return await _mapper.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mapper.GetById(id);
        
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] TrainerRequest request)
    {
        var result = await _mapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TrainerRequest request)
    {
        var result = await _mapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(id);
        return Ok(result);
    }
}