namespace Final_project.Models
{
    public interface IAccountViewModel
    {
        string Firstname { get; set; }
        string Lastname { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
        bool Terms { get; set; }
    }
}
