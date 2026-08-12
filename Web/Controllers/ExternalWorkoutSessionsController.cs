using System.Text.Json;
using Domain.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Service.Interface;

namespace Web.Controllers;

[Route("api/external/workoutSessions")]
[ApiController]
public class ExternalWorkoutSessionsController : ControllerBase
{
    private readonly IInboundWorkoutSessionEntryService _service;

    public ExternalWorkoutSessionsController(IInboundWorkoutSessionEntryService service)
    {
        _service = service;
    }

    [HttpPost]
    [EnableRateLimiting("external-api")]
    public async Task<IActionResult> ReceiveWorkoutSession([FromBody] InboundWorkoutSessionRequest request)
    {
        var apiClient = HttpContext.Items["ApiClient"] as ApiClient;
        if (apiClient is null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.MemberEmail))
            return BadRequest(new { error = "MemberEmail is required" });

        if (string.IsNullOrWhiteSpace(request.TrainerName))
            return BadRequest(new { error = "TrainerName is required" });

        if (string.IsNullOrWhiteSpace(request.ExerciseName))
            return BadRequest(new { error = "ExerciseName is required" });

        var payload = JsonSerializer.Serialize(request);
        var entry = await _service.CreateAsync(payload, apiClient.Id);

        return Accepted(new
        {
            id = entry.Id,
            status = "pending",
            message = "Workout session queued for processing"
        });
    }
    
    [HttpGet("{id}/status")]
    [EnableRateLimiting("external-api")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var entry = await _service.GetByIdNotNull(id);

        if (entry == null)
            return NotFound();
        
        return Ok(new
        {
            id = entry.Id,
            status = entry.Status.ToString().ToLower(),
            receivedAt = entry.ReceivedAt,
            processedAt = entry.ProcessedAt,
            createdWorkoutSessionId = entry.CreatedWorkoutSessionId,
            error = entry.ErrorMessage
        });
    }
}