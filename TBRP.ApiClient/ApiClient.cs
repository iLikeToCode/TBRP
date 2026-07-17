using TBRP.ErlcAPI.ERLC.V1;
using TBRP.ErlcAPI.ERLC.V2;

namespace TBRP.ErlcAPI;

public class ApiClient(string apiKey)
{
    public readonly ErlcApiClientV1 ErlcV1 = new(apiKey);
    public readonly ErlcApiClientV2 ErlcV2 = new(apiKey);
    public readonly RobloxUserApiClientV1 RobloxUserV1 = new();
}
