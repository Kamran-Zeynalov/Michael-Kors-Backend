using System.ComponentModel.DataAnnotations;

namespace Final_project.Areas.Admin.Model
{
    public class RegisterViewModel
    {
        [StringLength(maximumLength: 15)]
        public string Name { get; set; }
        [StringLength(maximumLength: 15)]
        public string SurName { get; set; }
        [StringLength(maximumLength: 15)]
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
