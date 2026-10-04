using System.ComponentModel.DataAnnotations;

namespace Library_Automation.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta is required")]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
        //Giriş yapmamış biri korumalı sayfaya girmeye çalışınca login'e yönlendirilir.
    }
}
