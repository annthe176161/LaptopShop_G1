using System.ComponentModel.DataAnnotations;

namespace LaptopStore.API.ViewModels
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
