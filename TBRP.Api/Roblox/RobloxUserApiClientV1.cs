using RestSharp;
using TBRP.Api.Structs;

namespace TBRP.Api.Roblox;

public partial class RobloxUserApiClientV1
{
    private readonly ExtendedRestClient _client = new(
        new RestClientOptions("https://users.roblox.com/v1")
        {
        });
}
