namespace MuseoLibrary.ApplicationDomain.DTOs
{
    public class PasswordDto
    {
        public string? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
