namespace Service.Interface;

public interface IExcelExportService
{
    Task<byte[]> ExportToExcel(Guid memberId);
}