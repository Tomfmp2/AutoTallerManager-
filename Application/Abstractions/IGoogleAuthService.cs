namespace Application.Abstractions;

public class GoogleUserDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string GoogleId { get; set; } = string.Empty;
}

public interface IGoogleAuthService
{
    Task<GoogleUserDto?> ValidateGoogleTokenAsync(string idToken);
}
