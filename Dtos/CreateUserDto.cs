using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 2)]
        public string? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
