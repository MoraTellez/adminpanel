using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using adminpanel.Utils;

namespace adminpanel.Models.DTOS
{
    [PasswordsMatch]
    public class RegisterDTO
    {
        [Required(ErrorMessage = "The Name field is requried.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Password field is requried.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "The Confirm Password field is requried.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
