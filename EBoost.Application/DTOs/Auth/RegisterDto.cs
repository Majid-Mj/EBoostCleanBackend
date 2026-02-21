using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace EBoost.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="Full name is required")]
        [MinLength(3, ErrorMessage ="Full name must be at least 3 characters")]
        [MaxLength(100,ErrorMessage ="Full name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
                ErrorMessage ="Full name can contain only letters and spaces")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Invalid email format")]
        [MaxLength(150,ErrorMessage ="Email cannot exceed 150 characters")]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage ="Password is required")]
        [MinLength(8, ErrorMessage ="Password must be at least 8 characters")]
        [MaxLength(100, ErrorMessage ="Password cannot exceed 100 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; } = null!;


        [Required(ErrorMessage ="Comfirm password is required")]
        [Compare("Password", ErrorMessage ="Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;    

    }
}
