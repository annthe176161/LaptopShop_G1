using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaptopStore.API.ViewModels
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required!")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
    }
}
