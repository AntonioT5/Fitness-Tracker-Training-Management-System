using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WorkoutPlanController : ControllerBase
{
    private readonly WorkoutPlanMapper _mapper;
    
    public WorkoutPlanController(WorkoutPlanMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<WorkoutPlanResponse?>> GetAll()
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
    public async Task<PaginatedResponse<WorkoutPlanResponse?>> Page([FromQuery] PaginateRequest pageRequest)
    {
        return await _mapper.PaginatedGetAllAsync(pageRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] WorkoutPlanRequest request)
    {
        try
        {
            var result = await _mapper.InsertAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new {error = e.Message});
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] WorkoutPlanRequest request)
    {
        try
        {
            var result = await _mapper.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (InvalidOperationException e) when (e.Message.Contains("WorkoutPlan"))
        {
            return NotFound(new {error = e.Message});
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new {error = e.Message});
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