using ClosedXML.Excel;
using Domain.Dto;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ExcelImportService : IExcelImportService
{
    private readonly IRepository<WorkoutSession> _workoutSessionRepository;
    private readonly IMemberService _memberService;
    private readonly ITrainerService _trainerService;
    private readonly IExerciseService _exerciseService;

    public ExcelImportService(IRepository<WorkoutSession> wRepository, IMemberService memberService, ITrainerService trainerService, IExerciseService exerciseService)
    {
        _workoutSessionRepository = wRepository;
        _memberService = memberService;
        _trainerService = trainerService;
        _exerciseService = exerciseService;
    }

    public async Task<ImportResult<WorkoutSessionImportDto>> ImportEventsAsync(Stream fileStream)
    {
        var result = new ImportResult<WorkoutSessionImportDto>();

        using var workbook = new XLWorkbook(fileStream);
        
        var ws = workbook.Worksheet(1);

        var expectedHeaders = new Dictionary<string, int>();
        var headerRow = ws.Row(1);
        
        var lastHeaderCell = headerRow.LastCellUsed();

        if (lastHeaderCell == null)
        {
            result.Errors.Add(new ImportError
            {
                Row = 1,
                Column = "Header",
                Message = "The Excel file is empty. No headers were found."
            });

            return result;
        }
        
        for (int col = 1; col <= headerRow.LastCellUsed().Address.ColumnNumber; col++)
        {
            expectedHeaders[headerRow.Cell(col).GetString().Trim().ToLower()] = col;
        }

        var requiredHeaders = new[]
        {
            "member", "date", "exercise", "trainer", "setscompleted", "repscompleted",
            "weightusedkg", "durationminutes", "notes"
        };
        foreach (var h in requiredHeaders)
        {
            if (!expectedHeaders.ContainsKey(h))
            {
                result.Errors.Add(new ImportError
                {
                    Row = 1,
                    Column = h,
                    Message = $"Missing required column: '{h}'"
                });
            }
        }
        
        if (result.HasErrors) 
            return result;
        
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        result.TotalRows = lastRow - 1;
        
        var memberCol = expectedHeaders["member"];
        var trainerCol = expectedHeaders["trainer"];
        var exerciseCol = expectedHeaders["exercise"];
        
        var requestedMemberEmails = Enumerable.Range(2, lastRow - 1)
            .Select(r => ws.Cell(r, memberCol).GetString().Trim())
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var requestedTrainerNames = Enumerable.Range(2, lastRow - 1)
            .Select(r => ws.Cell(r, trainerCol).GetString().Trim())
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var requestedExerciseNames = Enumerable.Range(2, lastRow - 1)
            .Select(r => ws.Cell(r, exerciseCol).GetString().Trim())
            .Where(v => !string.IsNullOrEmpty(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        var memberMap = new Dictionary<string, Member>(StringComparer.OrdinalIgnoreCase);
        foreach (var email in requestedMemberEmails)
        {
            var member = await _memberService.GetByEmailAsync(email);
            if (member != null) memberMap[email] = member;
        }

        var trainerMap = new Dictionary<string, Trainer>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in requestedTrainerNames)
        {
            var trainer = await _trainerService.GetByNameAsync(name);
            if (trainer != null) trainerMap[name] = trainer;
        }

        var exerciseMap = new Dictionary<string, Exercise>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in requestedExerciseNames)
        {
            var exercise = await _exerciseService.GetByNameAsync(name);
            if (exercise != null) exerciseMap[name] = exercise;
        }

        for (int row = 2; row <= lastRow; row++)
        {
            var memberEmail = ws.Cell(row, expectedHeaders["member"]).GetString().Trim();
            if (string.IsNullOrEmpty(memberEmail) || !memberMap.TryGetValue(memberEmail, out var member))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "Member", Message = $"Member '{memberEmail}' not found" });
                continue;
            }

            if (!ws.Cell(row, expectedHeaders["date"]).TryGetValue(out DateTime date))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "Date", Message = "Invalid date format" });
                continue;
            }

            var exerciseName = ws.Cell(row, expectedHeaders["exercise"]).GetString().Trim();
            if (string.IsNullOrEmpty(exerciseName) || !exerciseMap.TryGetValue(exerciseName, out var exercise))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "Exercise", Message = $"Exercise '{exerciseName}' not found" });
                continue;
            }

            var trainerName = ws.Cell(row, expectedHeaders["trainer"]).GetString().Trim();
            if (string.IsNullOrEmpty(trainerName) || !trainerMap.TryGetValue(trainerName, out var trainer))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "Trainer", Message = $"Trainer '{trainerName}' not found" });
                continue;
            }

            if (!ws.Cell(row, expectedHeaders["setscompleted"]).TryGetValue(out int setsCompleted))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "SetsCompleted", Message = "Invalid number" });
                continue;
            }

            if (!ws.Cell(row, expectedHeaders["repscompleted"]).TryGetValue(out int repsCompleted))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "RepsCompleted", Message = "Invalid number" });
                continue;
            }

            if (!ws.Cell(row, expectedHeaders["weightusedkg"]).TryGetValue(out decimal weightUsedKg))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "WeightUsedKg", Message = "Invalid number" });
                continue;
            }

            if (!ws.Cell(row, expectedHeaders["durationminutes"]).TryGetValue(out int durationMinutes))
            {
                result.Errors.Add(new ImportError
                    { Row = row, Column = "DurationMinutes", Message = "Invalid number" });
                continue;
            }

            result.SuccessfulRecords.Add(new WorkoutSessionImportDto
            {
                MemberId = member.Id,
                TrainerId = trainer.Id,
                ExerciseId = exercise.Id,
                Date = date,
                SetsCompleted = setsCompleted,
                RepsCompleted = repsCompleted,
                WeightUsedKg = weightUsedKg,
                DurationMinutes = durationMinutes,
                Notes = expectedHeaders.ContainsKey("notes")
                    ? ws.Cell(row, expectedHeaders["notes"]).GetString()
                    : null
            });
        }
        foreach (var dto in result.SuccessfulRecords)
        {
            await _workoutSessionRepository.InsertAsync(new WorkoutSession
            {
                MemberId = dto.MemberId,
                TrainerId = dto.TrainerId,
                ExerciseId = dto.ExerciseId,
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                SetsCompleted = dto.SetsCompleted,
                RepsCompleted = dto.RepsCompleted,
                WeightUsedKg = dto.WeightUsedKg,
                DurationMinutes = dto.DurationMinutes,
                Notes = dto.Notes
            });
        }
        return result;
    }
}