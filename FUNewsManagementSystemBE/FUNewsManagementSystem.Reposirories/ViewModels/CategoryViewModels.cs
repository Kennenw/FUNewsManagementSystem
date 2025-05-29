using FUNewsManagementSystem.Reposirories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{
    internal class CategoryViewModels
    {

    }

    public class CreateCategoryViewModels
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; } = true;
    }

    public class UpdateCategoryViewModels
    {
        [MaxLength(100)]
        public string? CategoryName { get; set; } = null!;
        [MaxLength(250)]
        public string? CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}
