using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.Models;

public partial class SystemAccount
{
    [Key]
    [Required]
    public short AccountId { get; set; }
    [MaxLength(100)]
    public string? AccountName { get; set; }
    [MaxLength(70)]
    public string? AccountEmail { get; set; }

    public int? AccountRole { get; set; }
    [MaxLength(70)]
    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
