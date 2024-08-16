using System.ComponentModel.DataAnnotations;

namespace Final_project.Entities
{
    public class AccountDetail
    {
        [Key]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
