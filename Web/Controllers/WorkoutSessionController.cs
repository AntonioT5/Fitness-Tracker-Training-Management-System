using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkoutSessionController : ControllerBase
{
    private readonly WorkoutSessionMapper _mapper;

    public WorkoutSessionController(WorkoutSessionMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<WorkoutSessionResponse?>> GetAll()
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
    
    [HttpGet("paged")]
    public async Task<PaginatedResponse<WorkoutSessionResponse?>> Page([FromQuery] PaginateRequest pageRequest)
    {
        return await _mapper.PaginatedGetAllAsync(pageRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] WorkoutSessionRequest request)
    {
        try
        {
            var result = await _mapper.InsertAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] WorkoutSessionRequest request)
    {
        try
        {
            var result = await _mapper.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    
}