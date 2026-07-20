using RestSharp;
using TBRP.Api.Structs;

namespace TBRP.Api.Roblox;

partial class RobloxUserApiClientV1
{
    public async Task<RobloxUser[]?> GetUsersByUsername(string username)
    {
        var response = await GetUsersByUsernames([username]);
        return response;
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
        
        return response.Data.Length == 0 ? [] : response.Data;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class GetRobloxUsersByUsernamesResponse
{
    public required RobloxUser[] Data { get; init; }
}

