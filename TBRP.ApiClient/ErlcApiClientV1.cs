using RestSharp;
using RestSharp.Authenticators;

namespace TBRP.ErlcAPI;

public partial class ErlcApiClientV1(string apiKey)
{
    private readonly RestClient _client = new(new RestClientOptions("https://api.erlc.gg/v1")
    {
        Authenticator = new ErlcApiV1Authenticator(apiKey)
    });
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