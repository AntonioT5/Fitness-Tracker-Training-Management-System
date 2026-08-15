using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IExcelExportService _excelExportService;
    private readonly IMemberService _memberService;
    
    public ExportController(IExcelExportService excelExportService, IMemberService memberService)
    {
        _excelExportService = excelExportService;
        _memberService = memberService;
    }

    [HttpGet("{memberId}/workoutSession")]
    public async Task<IActionResult> ExportAsync(Guid memberId)
    {
        try
        {
            _ = await _memberService.GetByIdNotNullAsync(memberId);
        }
        catch (InvalidOperationException)
        {
            return NotFound("Member not found");
        }
        
        var bytes = await _excelExportService.ExportToExcel(memberId);
        
        return File(
            bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"workoutSession-{memberId}.xlsx"
        );
    }
}