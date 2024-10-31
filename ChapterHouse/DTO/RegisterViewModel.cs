using System.ComponentModel.DataAnnotations;

namespace ChapterHouse.Models
{
    public class RegisterViewModel
    {
        

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

      
    }
}
