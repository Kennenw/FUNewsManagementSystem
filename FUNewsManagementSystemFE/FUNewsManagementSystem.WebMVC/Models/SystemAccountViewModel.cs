using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public partial class SystemAccountViewModel
    {
        public short AccountId { get; set; }
        [MaxLength(100)]
        public string? AccountName { get; set; }
        [MaxLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }
        [MaxLength(70)]
        public string? AccountPassword { get; set; }

    }

    public class CreateSystemAccountViewModel
    {
        public string? AccountName { get; set; }
        [MaxLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }
        [MaxLength(70)]
        public string? AccountPassword { get; set; }
    }

    public class UpdateSystemAccountViewModel
    {
        public string? AccountName { get; set; }
        [MaxLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }
        [MaxLength(70)]
        public string? AccountPassword { get; set; }
    }
}
