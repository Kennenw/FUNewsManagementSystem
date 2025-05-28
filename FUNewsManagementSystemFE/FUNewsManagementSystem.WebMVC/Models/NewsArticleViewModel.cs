namespace FUNewsManagementSystem.WebMVC.Models
{
    public class NewsArticleViewModel
    {
        public string NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int CategoryId { get; set; }
        public bool NewsStatus { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Category { get; set; }
        public string CreatedBy { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }


    public class NewsArticleListViewModel
    {
        public List<NewsArticleViewModel> Articles { get; set; } = new List<NewsArticleViewModel>();
        public string SearchQuery { get; set; } = string.Empty;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
