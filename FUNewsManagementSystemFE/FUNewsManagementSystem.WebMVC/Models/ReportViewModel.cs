using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<NewsReportItem> NewsItems { get; set; } = new List<NewsReportItem>();
    }

    public class NewsReportItem
    {
        public string NewsArticleId { get; set; }
        public string? Title { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
    }
   
}
