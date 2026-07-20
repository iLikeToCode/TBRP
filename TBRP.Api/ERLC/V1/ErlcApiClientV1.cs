using RestSharp;
using RestSharp.Authenticators;
using TBRP.Api.Structs;

namespace TBRP.Api.ERLC.V1;

public partial class ErlcApiClientV1
{
    private readonly ExtendedRestClient _client;

    public ErlcApiClientV1(string apiKey)
    {
        _client = new ExtendedRestClient(
            new RestClientOptions("https://api.erlc.gg/v1")
            {
                Authenticator = new ErlcApiV1Authenticator(apiKey)
            });
    }
}

internal class ErlcApiV1Authenticator(string apiKey) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        request.AddHeader("server-key", apiKey);
        return ValueTask.CompletedTask;
    }
}
