using ChapterHouse.DTO;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChapterHouse.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult signup()
        {
            return View();
        }
        public IActionResult signin()
        {
            return View();
        }

        public async Task<IActionResult> SigninUser(LogInViewModel logInViewModel)
        {
            if (ModelState.IsValid)
            {
                await _authService.LogInUser(logInViewModel);
             return   RedirectToAction("Index", "Home");
            }
            return View("signin", logInViewModel);
        }

        public async Task<IActionResult> SignupUser(RegisterViewModel RegisterViewModel)
        {
            if (ModelState.IsValid)
            {
                await _authService.RegisterUser(RegisterViewModel);
              return  RedirectToAction("Index", "Home");
            }
            return View("signup", RegisterViewModel);
        }

        public async Task<IActionResult> SignoutUser()
        {
          
                await _authService.LogOutUser();
                return RedirectToAction("Index", "Home");
           
         
        }
    }
}
