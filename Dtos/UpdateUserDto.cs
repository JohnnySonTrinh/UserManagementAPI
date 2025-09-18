using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Dtos
{
    public class UpdateUserDto
    {

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "First name cannot be empty or whitespace.")]
    public string FirstName { get; set; } = string.Empty;


    [Required]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Last name cannot be empty or whitespace.")]
    public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Department cannot be whitespace.")]
    public string? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
