using TBRP.Api.Roblox;
using ErlcApiClientV1 = TBRP.Api.ERLC.V1.ErlcApiClientV1;
using ErlcApiClientV2 = TBRP.Api.ERLC.V2.ErlcApiClientV2;

namespace TBRP.Api;

public class ApiClient(string apiKey)
{
    public readonly ErlcApiClientV1 ErlcV1 = new(apiKey);
    public readonly ErlcApiClientV2 ErlcV2 = new(apiKey);
    public readonly RobloxUserApiClientV1 RobloxUserV1 = new();
}
