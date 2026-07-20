using RestSharp;
using TBRP.Api.Structs;

namespace TBRP.Api.Roblox;

partial class RobloxUserApiClientV1
{
    public async Task<RobloxUser[]> SearchUsersByUsername(string username, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return [];
        }

        var request = new RestRequest("users/search");
        request.AddQueryParameter("keyword", username.Trim());
        request.AddQueryParameter("limit", Math.Clamp(limit, 1, 25));

        var response = await _client.GetAsync<SearchRobloxUsersByUsernameResponse>(request);

        return response.Data.Length == 0 ? [] : response.Data;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class SearchRobloxUsersByUsernameResponse
{
    public required RobloxUser[] Data { get; init; }
}
