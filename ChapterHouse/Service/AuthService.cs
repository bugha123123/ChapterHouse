using ChapterHouse.ApplicationDbContext;
using ChapterHouse.DTO;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace ChapterHouse.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContextion _dbcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, AppDbContextion dbcontext, IWebHostEnvironment hostingEnvironment, IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _dbcontext = dbcontext;
            _hostingEnvironment = hostingEnvironment;
            _passwordHasher = passwordHasher;
        }
        public async Task<User?> GetLoggedInUserAsync()
        {
            // Check if the user is authenticated and has a name
            if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                // User is not authenticated, return null
                return null;
            }

            // Get the user name
            string? userName = _httpContextAccessor.HttpContext.User.Identity?.Name;

            // Check if the user name is null
            if (userName == null)
            {

                return null;
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(userName);

            return user;
        }



        
        public async Task RegisterUser(RegisterViewModel registerViewModel)
        {
            var user = new User { UserName = registerViewModel.Email, Email = registerViewModel.Email };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                
            }
        }

        public async Task LogInUser(LogInViewModel logInViewModel)
        {
            if (logInViewModel == null)
            {
                throw new ArgumentNullException(nameof(logInViewModel), "LogInViewModel cannot be null");
            }

            // Attempt to sign in the user using their email and password
            var result = await _signInManager.PasswordSignInAsync(logInViewModel.Email, logInViewModel.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                // Handle failed login attempt (optional: log the failure or throw an exception)
                throw new InvalidOperationException("Invalid login attempt.");
            }
        }

        public async Task LogOutUser()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
