
using FUNewsManagementSystem.Reposirories.Models;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{
    public class SystemAccountViewModels
    {
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





    public class AdminAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string AccountId { get; set; }
    }

    public class LoginRequestViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }    
    }
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string AccountId { get; set; }
    }
}
