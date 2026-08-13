using ClosedXML.Excel;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IExcelImportService _excelImportService;
    private readonly IWorkoutSessionService _workoutSessionService;

    public ImportController(IExcelImportService excelImportService, IWorkoutSessionService workoutSessionService)
    {
        _excelImportService = excelImportService;
        _workoutSessionService = workoutSessionService;
    }
    
    [HttpPost("workoutSession")]
    public async Task<IActionResult> ImportEvents(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".xlsx")
            return BadRequest("Only .xlsx supported");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File exceeds 5 MB");

        using var stream = file.OpenReadStream();
        var result = await _excelImportService.ImportEventsAsync(stream);

        if (result.HasErrors)
        {
            return Ok(new
            {
                success = false,
                totalRows = result.TotalRows,
                successCount = result.SuccessfulRecords.Count,
                errorCount = result.Errors.Count,
                errors = result.Errors
            });
        }

        var workoutSession = result.SuccessfulRecords.Select(dto => new WorkoutSession
        {
            Date = dto.Date,
            SetsCompleted = dto.SetsCompleted,
            RepsCompleted = dto.RepsCompleted,
            WeightUsedKg = dto.WeightUsedKg,
            DurationMinutes =  dto.DurationMinutes,
            Notes = dto.Notes,
            MemberId =  dto.MemberId,
            TrainerId =   dto.TrainerId,
            ExerciseId =  dto.ExerciseId
        }).ToList();

        await _workoutSessionService.AddRangeAsync(workoutSession);

        return Ok(new
        {
            success = true,
            totalRows = result.TotalRows,
            createdCount = workoutSession.Count
        });
    }
    
    [HttpGet("workoutSession/get-import-template")]
    public IActionResult GetImportTemplate()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("WorkoutSession Import");

        var headers = new[]
        {
            "Date", "Exercise", "Trainer",
            "Sets Completed", "Reps Completed", "Weight Used (kg)",
            "Duration (minutes)", "Notes"
        };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }
        
        ws.Cell(2, 1).Value = new DateTime(2026, 7, 15, 18, 0, 0);
        ws.Cell(2, 2).Value = "Exercise Name";
        ws.Cell(2, 3).Value = "Trainer Name";
        ws.Cell(2, 4).Value = 5;
        ws.Cell(2, 5).Value = 10;
        ws.Cell(2, 6).Value = 70;
        ws.Cell(2, 7).Value = 3;
        ws.Cell(2, 8).Value = "Note Test";

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "workoutSession-import-template.xlsx");
    }

}