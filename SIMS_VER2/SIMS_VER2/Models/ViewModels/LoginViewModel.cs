using System.ComponentModel.DataAnnotations;

namespace SIMS_VER2.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter username")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}

