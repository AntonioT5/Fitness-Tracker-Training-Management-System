using ClosedXML.Excel;
using Service.Interface;

namespace Service.Implementation;

public class ExcelExportService : IExcelExportService
{
    private readonly IWorkoutSessionService _workoutSessionService;

    public ExcelExportService(IWorkoutSessionService workoutSessionService)
    {
        _workoutSessionService = workoutSessionService;
    }

    public async Task<byte[]> ExportToExcel(Guid memberId)
    {
        var workoutSessions = await _workoutSessionService.GetAllByMemberNameAsync(memberId);
        
        using var workbook = new XLWorkbook();
        
        var ws = workbook.Worksheets.Add("WorkoutSessions");
        
        var headers = new[]
        {
            "WorkoutSession ID", "Date", "Exercise", "Trainer",
            "Sets Completed", "Reps Completed", "Weight Used (kg)",
            "Duration (minutes)", "Notes"
        };
        
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i+1).Value = headers[i];
        }
        
        var headerRange = ws.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int row = 2;
        
        foreach (var workoutSession in workoutSessions)
        {
            ws.Cell(row, 1).Value = workoutSession.Date;
            ws.Cell(row, 2).Value = workoutSession.Exercise?.Name ?? "—";
            ws.Cell(row, 3).Value = workoutSession.Trainer?.User != null
                ? $"{workoutSession.Trainer.User.FirstName} {workoutSession.Trainer.User.LastName}"
                : "—";
            ws.Cell(row, 4).Value = workoutSession.SetsCompleted;
            ws.Cell(row, 5).Value = workoutSession.RepsCompleted;
            ws.Cell(row, 6).Value = workoutSession.WeightUsedKg;
            ws.Cell(row, 7).Value = workoutSession.DurationMinutes;
            ws.Cell(row, 8).Value = workoutSession.Notes;
            
            row++;
        }
        
        ws.Column(1).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";

        ws.Columns().AdjustToContents();
        
        ws.RangeUsed()?.SetAutoFilter();
        
        ws.SheetView.FreezeRows(1);
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}