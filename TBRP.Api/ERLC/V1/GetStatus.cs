using System.Text.Json.Serialization;
using RestSharp;

namespace TBRP.Api.ERLC.V1;

partial class ErlcApiClientV1
{
    public async Task<ErlcServerStatus> GetStatus()
    {
        var request = new RestRequest("server");

        var response = await _client.GetAsync<ErlcServerStatus>(request);

        return response ?? throw new Exception("Response is null.");
    }
}

public sealed class ErlcServerStatus
{
    [JsonPropertyName("Name")]
    public required string Name { get; init; }

    [JsonPropertyName("OwnerId")]
    public required long OwnerId { get; init; }

    [JsonPropertyName("CoOwnerIds")]
    public required long[] CoOwners { get; init; }

    [JsonPropertyName("CurrentPlayers")]
    public required int PlayerCount { get; init; }

    [JsonPropertyName("MaxPlayers")]
    public required int MaxPlayers { get; init; }

    [JsonPropertyName("JoinKey")]
    public required string JoinCode { get; init; }

    [JsonPropertyName("AccVerifiedReq")]
    public required string AccountVerificationRequirement { get; init; }

    [JsonPropertyName("TeamBalance")]
    public required bool TeamBalance { get; init; }
}