using System.ComponentModel.DataAnnotations;

namespace AuthApp.Dto
{
    public class ChangePasswordDto
    {
        [Required]
        [StringLength(100)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
