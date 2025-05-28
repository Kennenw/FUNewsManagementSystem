using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
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
}
