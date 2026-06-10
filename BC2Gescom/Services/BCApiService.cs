using FabToolKit.Api;

namespace BC2Gescom.Services;

internal class BCApiService : ApiServiceBase
{
    private BCApiInformation _apiConfig;

    public BCApiService(string baseUrl, HttpClient httpClient, BCApiInformation apiConfig, ApiAuthConfig? auth = null) : base(baseUrl, httpClient, auth)
    {
        _apiConfig = apiConfig;
    }

}
