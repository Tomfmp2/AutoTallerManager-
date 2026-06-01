using Application.Abstractions;
using System.Net.Http.Json;

namespace Infrastructure.Identity;

public class GoogleAuthService : IGoogleAuthService
{
    private static readonly HttpClient _httpClient = new();

    public async Task<GoogleUserDto?> ValidateGoogleTokenAsync(string accessToken)
    {
        try
        {
            // Use Google's userinfo endpoint with the access_token
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
                return null;

            return new GoogleUserDto
            {
                Email = userInfo.Email,
                FirstName = userInfo.GivenName ?? string.Empty,
                LastName = userInfo.FamilyName ?? string.Empty,
                GoogleId = userInfo.Sub ?? string.Empty
            };
        }
        catch
        {
            return null;
        }
    }

    private class GoogleUserInfo
    {
        public string? Sub { get; set; }
        public string? Email { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }
    }
}
