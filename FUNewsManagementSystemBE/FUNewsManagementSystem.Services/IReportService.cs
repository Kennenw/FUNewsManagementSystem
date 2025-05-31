
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface IReportService
    {
        Task<ReportViewModel> GetNewsReport(DateTime startDate, DateTime endDate);
    }
}
