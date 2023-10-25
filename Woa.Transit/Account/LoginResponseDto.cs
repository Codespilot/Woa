using Newtonsoft.Json;

namespace Woa.Transit;

public class LoginResponseDto
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    public int Id { get; set; }

    public string Username { get; set; }

    [JsonProperty("expires_at")]
    public long ExpiresAt { get; set; }
}