using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExerciseWorkoutPlanController : ControllerBase
{
    private readonly ExerciseWorkoutPlanMapper _mapper;
    
    public ExerciseWorkoutPlanController(ExerciseWorkoutPlanMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<ExerciseWorkoutPlanResponse?>> GetAll()
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
    public async Task<PaginatedResponse<ExerciseWorkoutPlanResponse?>> Page([FromQuery] PaginateRequest pageRequest)
    {
        return await _mapper.PaginatedGetAllAsync(pageRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] ExerciseWorkoutPlanRequest request)
    {
        try
        {
            var result = await _mapper.InsertAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { error  = e.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ExerciseWorkoutPlanRequest request)
    {
        try
        {
            var result = await _mapper.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (InvalidOperationException e) when (e.Message.Contains("ExerciseWorkoutPlan"))
        {
            return NotFound();
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