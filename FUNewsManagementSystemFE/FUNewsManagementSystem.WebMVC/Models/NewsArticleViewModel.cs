using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class NewsArticleViewModel
    {
        public string NewsArticleId { get; set; } = null!;
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [Required, MaxLength(150)]
        public string Headline { get; set; } = null!;
        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool NewsStatus { get; set; } = true;
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public List<TagsViewModels> Tags { get; set; } = new List<TagsViewModels>();
        public CategoryViewModel? Category { get; set; }
        public SystemAccountViewModel? CreatedBy { get; set; }
    }


    public class CreateNewsArticleViewModels
    {
        [Required, MaxLength(150)]
        public string Headline { get; set; } = null!;
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool NewsStatus { get; set; } = true; 
        public short? CreatedById { get; set; }
        [Display(Name = "Tags")]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Tags { get; set; }
        public IEnumerable<SelectListItem>? Accounts { get; set; }
    }


    public class UpdateNewsArticleViewModels
    {
        [Required, MaxLength(20)]
        public string NewsArticleId { get; set; } = null!;
        [Required, MaxLength(150)]
        public string Headline { get; set; } = null!;
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool NewsStatus { get; set; } = true; 
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Tags")]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public IEnumerable<CategoryViewModel>? Categories { get; set; }
        public IEnumerable<TagsViewModels>? Tags { get; set; }
        public IEnumerable<SystemAccountViewModel>? Accounts { get; set; }
    }


    public class ListViewModel<T>
    {
        public List<T> Item { get; set; } = new List<T>();
        public string SearchQuery { get; set; } = string.Empty;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }


    public class ArticleHistoryViewModel
    {
        [Required]
        [Range(1, short.MaxValue, ErrorMessage = "Account ID must be a positive number.")]
        public short AccountId { get; set; }

        public List<NewsArticleHistoryViewModels>? Articles { get; set; } = new List<NewsArticleHistoryViewModels>();
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
        public bool NewsStatus { get; set; } = true;
    }
}
