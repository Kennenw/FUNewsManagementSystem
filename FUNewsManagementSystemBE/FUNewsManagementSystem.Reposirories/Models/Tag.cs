using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.Models;

public partial class Tag
{
    [Key]
    public int TagId { get; set; }
    [MaxLength(50)]
    public string? TagName { get; set; }
    [MaxLength(400)]
    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
