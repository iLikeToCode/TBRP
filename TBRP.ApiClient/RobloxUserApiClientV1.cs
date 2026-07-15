using RestSharp;
using RestSharp.Authenticators;

namespace TBRP.ErlcAPI;

public partial class RobloxUserApiClientV1()
{
    private readonly RestClient _client = new(new RestClientOptions("https://users.roblox.com/v1")
    {});
}