using RestSharp;
using TBRP.Api.Structs;

namespace TBRP.Api.Roblox;

partial class RobloxUserApiClientV1
{
    public async Task<RobloxUser?> GetUserById(long id)
    {
        var response = await GetUsersByIds([id]);
        return response[0] ?? null;
    }
    
    public async Task<RobloxUser[]> GetUsersByIds(long[] ids)
    {
        var request = new RestRequest("users");
        request.AddJsonBody(new
        {
            userIds = ids,
            excludeBannedUsers = true
        });

        var response = await _client.PostAsync<GetRobloxUsersByIdsResponse>(request);

        return response.Data.Length == 0 ? [] : response.Data;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class GetRobloxUsersByIdsResponse
{
    public required RobloxUser[] Data { get; init; }
}