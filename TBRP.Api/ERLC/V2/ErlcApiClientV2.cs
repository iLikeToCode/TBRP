using RestSharp;
using RestSharp.Authenticators;
using TBRP.Api.Structs;

namespace TBRP.Api.ERLC.V2;

public partial class ErlcApiClientV2
{
    private readonly ExtendedRestClient _client;

    public ErlcApiClientV2(string apiKey)
    {
        _client = new ExtendedRestClient(
            new RestClientOptions("https://api.erlc.gg/v2")
            {
                Authenticator = new ErlcApiV2Authenticator(apiKey)
            });
    }
}

internal class ErlcApiV2Authenticator(string apiKey) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        request.AddHeader("server-key", apiKey);
        return ValueTask.CompletedTask;
    }
}
