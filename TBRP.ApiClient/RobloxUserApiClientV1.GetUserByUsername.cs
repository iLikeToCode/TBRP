using RestSharp;
using TBRP.ErlcAPI.Structs;

namespace TBRP.ErlcAPI;

partial class RobloxUserApiClientV1
{
    public async Task<RobloxUser> GetUserByUsername(string username)
    {
        var response = await GetUsersByUsernames([username]);
        return response[0];
    }
    
    public async Task<RobloxUser[]> GetUsersByUsernames(string[] usernames)
    {
        var request = new RestRequest("usernames/users");
        request.AddJsonBody(new
        {
            usernames,
            excludeBannedUsers = true
        });

        var response = await _client.PostAsync<GetRobloxUsersByUsernamesResponse>(request);
        
        if (response == null || response.Data.Length == 0) throw new Exception("Response is null.");

        return response.Data;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class GetRobloxUsersByUsernamesResponse
{
    public required RobloxUser[] Data { get; init; }
}

