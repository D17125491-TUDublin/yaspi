using yaspi.common.OAuth;

namespace yaspi.integration.facebook;

public interface IFacebookApiService
{
    public string GetAuthRequestUrl();
    public KeyValuePair<string,string>[] GetConnectionData(OauthBearer token);
}