
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{

    public class UpdateNewsArticleViewModels
    {
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [MaxLength(150)]
        public string? Headline { get; set; } = null!;

        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; } = true;

        public short? UpdatedById { get; set; }
        public List<int>? Tags { get; set; } = new List<int>();
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
    }

    public class CreateNewsArticleViewModels
    {
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [Required]
        [MaxLength(150)]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; } = true;
        public List<int>? TagIds { get; set; } = new List<int>();

        public short? CreatedById { get; set; }
    }

    public class NewsArticleHistoryViewModels
    {
        public string? NewsArticleId { get; set; }
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<string>? TagName { get; set; } = new List<string>();
        public bool? NewsStatus { get; set; }
    }
}