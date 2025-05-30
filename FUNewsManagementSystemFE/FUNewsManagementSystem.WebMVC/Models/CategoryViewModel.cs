namespace FUNewsManagementSystem.WebMVC.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesciption { get; set; }
        public int ParentCategoryId { get; set; }
    }
}
