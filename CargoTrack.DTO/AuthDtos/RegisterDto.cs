
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.DTO.AuthDtos;

public class RegisterDto
{
    [Required]
    [StringLength(60)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
