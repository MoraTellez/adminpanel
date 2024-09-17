using adminpanel.Models.DTOS;
using System.ComponentModel.DataAnnotations;

namespace adminpanel.Utils
{
    public class PasswordsMatchAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dto = (RegisterDTO)validationContext.ObjectInstance;

            if (dto.Password != dto.ConfirmPassword)
            {
                return new ValidationResult("The passwords don't match.");
            }

            return ValidationResult.Success;
        }
    }
}
