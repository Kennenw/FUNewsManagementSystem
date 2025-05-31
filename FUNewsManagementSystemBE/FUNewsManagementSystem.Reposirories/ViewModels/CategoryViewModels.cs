
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{
    internal class CategoryViewModels
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CreateCategoryViewModels
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; } = null;

        public bool? IsActive { get; set; } = true;
    }

    public class UpdateCategoryViewModels
    {
        [MaxLength(100)]
        public string? CategoryName { get; set; } = null!;
        [MaxLength(250)]
        public string? CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; } = null;

        public bool? IsActive { get; set; }
    }
}
