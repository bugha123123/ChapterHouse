using ChapterHouse.ApplicationDbContext;
using ChapterHouse.DTO;
using ChapterHouse.Interface;
using ChapterHouse.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.Mail;
using System.Net;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;

namespace ChapterHouse.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppDbContextion _dbcontext;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, AppDbContextion dbcontext, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _dbcontext = dbcontext;
            _hostingEnvironment = hostingEnvironment;
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

          
        }

        public async Task LogOutUser()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task SendResurrectPasswordEmail(string email)
        {

            // used for sending emails to user's email
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("irakliberdzena314@gmail.com", "coca mmba ywsy lvyz");

                var timestamp = DateTime.UtcNow.ToString("o"); 
                var resetLink = $"https://localhost:7057/Auth/resetPassword?timestamp={timestamp}&email={email}";

                var htmlBody = $@"
<html>
<head>
    <style>
        .email-container {{
            font-family: Arial, sans-serif;
            background-color: #121212;
            color: #fff;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }}
        .button {{
            display: inline-block;
            background-color: #6b46c1;
            color: #fff;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            font-weight: bold;
            margin-top: 20px;
            transition: background-color 0.3s;
        }}
        .button:hover {{
            background-color: #805ad5;
        }}
        .info {{
            font-size: 0.9em;
            color: #b3b3b3;
            margin-top: 10px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>Forgot Your Spell?</h2>
        <p>No worries! Click the link below to reset your password. This link will expire in 5 minutes.</p>
        <a href='{resetLink}' class='button text-white'>Reset Password</a>
        <p class='info'>If you didn't request this, please ignore this email.</p>
    </div>
</body>
</html>
";
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("irakliberdzena314@gmail.com", "Spooky Bookstore");
                    message.To.Add(new MailAddress(email));
                    message.Subject = "Payment Receipt - Spooky Bookstore";
                    message.IsBodyHtml = true;
                    message.Body = htmlBody.ToString();

                    // Send the email
                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task ResetPassword(string email, DateTime EmailsendTime, string password)
        {
            // Check if the email send time is more than 5 minutes old
            if (DateTime.UtcNow - EmailsendTime > TimeSpan.FromMinutes(5))
            {
                throw new Exception("The password reset link has expired. Please request a new one.");
            }

            // Find the user by email
            var foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser == null)
            {
                throw new Exception("User not found.");
            }

            // Hash the new password
            var passwordHash = _userManager.PasswordHasher.HashPassword(foundUser, password);
            foundUser.PasswordHash = passwordHash;

            // Save changes to the database
            await _dbcontext.SaveChangesAsync();
        }


    }
}
