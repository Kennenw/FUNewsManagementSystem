using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.Models;

public partial class Category
{
    [Key]
    public short CategoryId { get; set; }
    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = null!;
    [Required]
    [MaxLength(250)]
    public string CategoryDesciption { get; set; } = null!;

    public short? ParentCategoryId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    public virtual Category? ParentCategory { get; set; }
}
