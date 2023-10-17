using System.Text.Json.Serialization;

namespace Woa.Transit;

public class LoginResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    public int Id { get; set; }

    public string Username { get; set; }

    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; set; }
}