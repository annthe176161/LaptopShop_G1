using System.ComponentModel.DataAnnotations;

namespace LaptopStore.API.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
