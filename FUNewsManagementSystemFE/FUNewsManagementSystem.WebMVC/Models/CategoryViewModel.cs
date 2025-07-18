﻿using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateCategoryViewModels
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;

        [MaxLength(250)]
        public string? CategoryDescription { get; set; } = "";

        public int? ParentCategoryId { get; set; } = null;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryViewModels
    {
        public int CategoryId { get; set; }
        [MaxLength(100)]
        public string? CategoryName { get; set; } = null!;
        [MaxLength(250)]
        public string? CategoryDescription { get; set; }

        public int? ParentCategoryId { get; set; } = null;

        public bool IsActive { get; set; } = true;
    }
}
