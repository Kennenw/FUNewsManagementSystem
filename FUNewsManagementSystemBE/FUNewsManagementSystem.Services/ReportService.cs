

using AutoMapper;
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public class ReportService : IReportService
    {
        public readonly IUnitOfWork _unitOfWork;
        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ReportViewModel> GetNewsReport(DateTime startDate, DateTime endDate)
        {
            var items = await _unitOfWork._newsArticleRepository.GetNewsReport(startDate, endDate);
            return new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                NewsItems = items
            };
        }
    }
}
