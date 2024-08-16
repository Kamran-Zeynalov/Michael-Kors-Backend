using Final_project.Entities;
using System.ComponentModel.DataAnnotations;

namespace Final_project.Models
{
    public class AccountViewModel
    {
        public User User { get; set; }
        public string Token { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
