
public class AuthResponse
{
    public UserInfo User { get; set; }
    public string AccessToken { get; internal set; }
    public string RefreshToken { get; internal set; }
    public DateTime ExpiresAt { get; internal set; }
}
