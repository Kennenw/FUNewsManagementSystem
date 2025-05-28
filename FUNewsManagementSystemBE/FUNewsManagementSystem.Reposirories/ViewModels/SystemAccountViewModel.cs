
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.Reposirories.ViewModels
{
    public class SystemAccountViewModel
    {
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
