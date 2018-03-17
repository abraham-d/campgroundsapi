using System.ComponentModel.DataAnnotations;

namespace campgrounds_api.DTOs
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password must be within 4 to 8 characters")]
        public string Password { get; set; }
    }
}