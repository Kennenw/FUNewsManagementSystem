using FUNewsManagementSystem.Reposirories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{
    internal class NewsArticleViewModels
    {
    }
    public class UpdateNewsArticleViewModels
    {
        [MaxLength(400)]
        public string? NewsTitle { get; set; }
        [Required]
        [MaxLength(150)]
        public string Headline { get; set; } = null!;

        [MaxLength(4000)]
        public string? NewsContent { get; set; }
        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; } = true;

        public short? UpdatedById { get; set; }

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

        public short? CreatedById { get; set; }
    }
}
