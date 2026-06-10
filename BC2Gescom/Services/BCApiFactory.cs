namespace BC2Gescom.Services;

public class BCApiFactory
{
    private HttpClient _httpClient;
    private BCApiInformation _apiConfig;

    public BCApiFactory(HttpClient httpClient, BCApiInformation apiConfig)
    {
        _httpClient = httpClient;
        _apiConfig = apiConfig;
    }

    internal BCApiService GetApi()
    {
        throw new NotImplementedException();
    }
}
