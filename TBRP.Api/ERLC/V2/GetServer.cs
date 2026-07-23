using System.Text.Json.Serialization;
using RestSharp;

namespace TBRP.Api.ERLC.V2;

partial class ErlcApiClientV2
{
    public async Task<ErlcServerResponse> GetServer()
    {
        var request = new RestRequest("server");

        var response = await _client.GetAsync<ErlcServerResponse>(request);

        return response ?? throw new Exception("Response is null.");
    }
}

public sealed class ErlcServerResponse
{
    public required string Name { get; set; }

    public required long OwnerId { get; set; }

    [JsonPropertyName("CoOwnerIds")]
    public required long[] CoOwners { get; set; }

    [JsonPropertyName("CurrentPlayers")]
    public required int PlayerCount { get; set; }

    public required int MaxPlayers { get; set; }

    [JsonPropertyName("JoinKey")]
    public required string JoinCode { get; set; }

    [JsonPropertyName("AccVerifiedReq")]
    public required string AccountVerificationRequirement { get; set; }

    public required bool TeamBalance { get; set; }
    
    public required ErlcPlayerResponse[] Players { get; set; }
    
}

public sealed class ErlcPlayerResponse
{
    public required string Team { get; set; }
    public required string Player { get; set; }
    public string? Callsign { get; set; }
    public required ErlcPlayerLocationResponse Location { get; set; }
    public required string Permission;
    public required int WantedStars;
}

public sealed class ErlcPlayerLocationResponse
{
    public required float LocationX { get; set; }
    public required float LocationY { get; set; }
    public string? PostalCode { get; set; }
    public string? StreetName { get; set; }
    public string? BuildingNumber { get; set; }
}