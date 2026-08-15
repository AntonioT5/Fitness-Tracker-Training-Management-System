using ClosedXML.Excel;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportController : ControllerBase
{
    private readonly IExcelImportService _excelImportService;

    public ImportController(IExcelImportService excelImportService)
    {
        _excelImportService = excelImportService;
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

        return Ok(new
        {
            success = !result.HasErrors,
            totalRows = result.TotalRows,
            createdCount = result.SuccessfulRecords.Count,
            errorCount = result.Errors.Count,
            errors = result.Errors
        });
    }
    
    [HttpGet("workoutSession/get-import-template")]
    public IActionResult GetImportTemplate()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("WorkoutSession Import");

        var headers = new[]
        {
            "member", "date", "exercise", "trainer", "setscompleted", "repscompleted",
            "weightusedkg", "durationminutes", "notes"
        };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }
        
        ws.Cell(2, 1).Value = "member@example.com";
        ws.Cell(2, 2).Value = new DateTime(2026, 7, 15, 18, 0, 0);
        ws.Cell(2, 3).Value = "Exercise Name";
        ws.Cell(2, 4).Value = "Trainer Name";
        ws.Cell(2, 5).Value = 5;
        ws.Cell(2, 6).Value = 10;
        ws.Cell(2, 7).Value = 70;
        ws.Cell(2, 8).Value = 3;
        ws.Cell(2, 9).Value = "Note Test";

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "workoutSession-import-template.xlsx");
    }

}