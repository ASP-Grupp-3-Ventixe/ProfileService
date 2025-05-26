using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string UserId { get; set; } = null!;
    
    [ProtectedPersonalData]
    public string? ImageUrl { get; set; }
    
    [Required]
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;
    
    [Required]
    [ProtectedPersonalData]
    public string LastName { get; set; } = null!;
    
    [Required]
    [ProtectedPersonalData]
    public string Initials { get; set; } = null!;
    
    [Required]
    [ProtectedPersonalData]
    public string Email { get; set; } = null!; 
    
    [Required]
    [ProtectedPersonalData]
    public string PhoneNumber { get; set; } = null!; 
    
    [ProtectedPersonalData]
    public string? JobTitle { get; set; }
    
    [ProtectedPersonalData]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    public virtual UserAddressEntity? Address { get; set; }
}

