using RestSharp;
using TBRP.ErlcAPI.Structs;

namespace TBRP.ErlcAPI;

public partial class RobloxUserApiClientV1
{
    private readonly ExtendedRestClient _client;

    public RobloxUserApiClientV1()
    {
        _client = new ExtendedRestClient(
            new RestClientOptions("https://users.roblox.com/v1")
            {
            });
    }
}
