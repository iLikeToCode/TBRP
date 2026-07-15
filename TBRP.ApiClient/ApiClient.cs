namespace TBRP.ErlcAPI;

public class ApiClient(string apiKey)
{
    public readonly ErlcApiClientV1 V1 = new(apiKey);
    public readonly RobloxUserApiClientV1 RobloxUserV1 = new();
}