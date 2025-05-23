namespace Presentation.Entities;

public class UserProfile
{
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!; // also in token but keep it here for consistency
    public string PhoneNumber { get; set; } = null!; 
    public string Initials { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}