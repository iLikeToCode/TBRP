using System.Diagnostics;
using System.Net;
using RestSharp;

namespace TBRP.ErlcAPI.Structs;

public class ExtendedRestClient
{
    private readonly RestClient _client;
    private readonly Uri? _baseUrl;

    public ExtendedRestClient(RestClientOptions options)
    {
        var url = Environment.GetEnvironmentVariable("PROXY_URL");
        options.Proxy = new WebProxy(url)
        {
            Credentials = new NetworkCredential("tbrp", "InsaneTBRP08642")
        };
        _client = new RestClient(options);
        _baseUrl = options.BaseUrl;
    }

    public Task<T> GetAsync<T>(
        RestRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Method = Method.Get;
        return ExecuteAsync<T>(request, cancellationToken);
    }

    public Task<T> PostAsync<T>(
        RestRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Method = Method.Post;
        return ExecuteAsync<T>(request, cancellationToken);
    }

    public async Task<T> ExecuteAsync<T>(
        RestRequest request,
        CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var requestName = GetRequestName(request);
            var stopwatch = Stopwatch.StartNew();

            Log($"HTTP --> {requestName}");

            RestResponse<T> response;
            try
            {
                response = await _client.ExecuteAsync<T>(request, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                stopwatch.Stop();
                Log($"HTTP !!  {requestName} threw after {stopwatch.ElapsedMilliseconds}ms: {exception.Message}");
                throw;
            }

            stopwatch.Stop();
            Log(
                $"HTTP <-- {requestName} status={(int)response.StatusCode} {response.StatusCode} restStatus={response.ResponseStatus} success={response.IsSuccessful} elapsed={stopwatch.ElapsedMilliseconds}ms");

            if (response.StatusCode != HttpStatusCode.TooManyRequests)
            {
                if (!response.IsSuccessful)
                {
                    var error = response.ErrorException?.Message
                                ?? response.ErrorMessage
                                ?? "No error message returned.";

                    Log($"HTTP xx  {requestName} failed: {error}");
                    LogFailureDetails(requestName, response);
                    throw new HttpRequestException(
                        $"Request failed with status code {(int)response.StatusCode}: {error}");
                }

                if (response.Data is null)
                {
                    Log($"HTTP xx  {requestName} returned no deserialized data.");
                    throw new InvalidOperationException("The response contained no data.");
                }

                return response.Data;
            }

            var resetHeader = response.Headers?
                .FirstOrDefault(h => h.Name?.Equals("X-RateLimit-Reset", StringComparison.OrdinalIgnoreCase) == true)
                ?.Value?.ToString();

            var delay = TimeSpan.FromSeconds(1);

            if (long.TryParse(resetHeader, out var resetUnix))
            {
                var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetUnix);
                delay = resetTime - DateTimeOffset.UtcNow;

                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero;
            }

            Log($"HTTP 429 {requestName}; retrying in {delay.TotalMilliseconds:N0}ms.");
            await Task.Delay(delay, cancellationToken);
        }
    }

    private string GetRequestName(RestRequest request)
    {
        var resource = string.IsNullOrWhiteSpace(request.Resource)
            ? "/"
            : request.Resource;

        if (_baseUrl is null)
            return $"{request.Method} {resource}";

        var baseUrl = _baseUrl.AbsoluteUri.TrimEnd('/');
        var relativeResource = resource.TrimStart('/');

        return $"{request.Method} {baseUrl}/{relativeResource}";
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] {message}");
    }

    private static void LogFailureDetails<T>(string requestName, RestResponse<T> response)
    {
        if (!string.IsNullOrWhiteSpace(response.Content))
            Log($"HTTP xx  {requestName} response body: {response.Content}");

        var relevantHeaders = response.Headers?
            .Where(header => header.Name is not null
                             && (header.Name.StartsWith("X-RateLimit", StringComparison.OrdinalIgnoreCase)
                                 || header.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase)
                                 || header.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)))
            .Select(header => $"{header.Name}={header.Value}")
            .ToArray();

        if (relevantHeaders is { Length: > 0 })
            Log($"HTTP xx  {requestName} headers: {string.Join(", ", relevantHeaders)}");
    }
}
