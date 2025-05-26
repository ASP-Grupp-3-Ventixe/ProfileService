using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;
    
    [ProtectedPersonalData]
    public string? StreetAddress { get; set; }
    
    [ProtectedPersonalData]
    public string? PostalCode { get; set; }
    
    [ProtectedPersonalData]
    public string? City { get; set; }
}