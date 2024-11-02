using ChapterHouse.DTO;
using ChapterHouse.Models;

namespace ChapterHouse.Interface
{
    public interface IAuthService
    {
        Task<User?> GetLoggedInUserAsync();
        Task RegisterUser(RegisterViewModel registerViewModel);
        Task LogInUser(LogInViewModel logInViewModel);
        Task LogOutUser();

        Task SendResurrectPasswordEmail(string email);

        Task ResetPassword(string email, DateTime EmailsendTime, string password);
    }
}
