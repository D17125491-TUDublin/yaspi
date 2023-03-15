namespace yaspi.integration.twitter;

using yaspi.common;

public class UserConnectedToTwitterEvent : IEvent
{
    public string UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserConnectedToTwitterEvent(string userId, string accessToken, string refreshToken)
    {
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}