using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Final_project.Models
{
    public class AccountDetailViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must contain only numbers")]
        public string? PhoneNumber { get; set; }

        [ValidateNever]
        public string? ErrorMessage { get; set; }
    }
}
