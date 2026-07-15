namespace TBRP.RobloxApi;

public class ErlcApiClient(string apiKey)
{
    public RobloxApiClientV1 V1 = new(apiKey);
}