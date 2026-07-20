using RestSharp;

namespace TBRP.Api.ERLC.V2;

partial class ErlcApiClientV2
{
    public async Task<string> SendCommand(string command)
    {
        var request = new RestRequest("server/command");
        request.AddJsonBody(
            new
            {
                command
            });

        var response = await _client.PostAsync<SendCommandResponse>(request);

        return response.Message;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public sealed class SendCommandResponse
{
    public required string Message { get; init; }
}