using RestSharp;
using TBRP.ErlcAPI.Structs;

namespace TBRP.ErlcAPI;

partial class RobloxUserApiClientV1
{
    public async Task<RobloxUser> GetUserById(long id)
    {
        var response = await GetUsersByIds([id]);
        return response[0];
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

        if (response == null || response.Data.Length == 0) throw new Exception("Response is null.");

        return response.Data;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class GetRobloxUsersByIdsResponse
{
    public required RobloxUser[] Data { get; init; }
}