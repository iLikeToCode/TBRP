using System.Diagnostics;
using System.Net;
using RestSharp;

namespace TBRP.Api.Structs;

public class ExtendedRestClient
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan MaxRateLimitDelay = TimeSpan.FromSeconds(30);

    private readonly RestClient _client;
    private readonly Uri? _baseUrl;

    public ExtendedRestClient(RestClientOptions options)
    {
        var url = Environment.GetEnvironmentVariable("PROXY_URL");
        if (!string.IsNullOrWhiteSpace(url))
        {
            options.Proxy = new WebProxy(url)
            {
                Credentials = new NetworkCredential("tbrp", "InsaneTBRP08642")
            };
        }

        if (options.Timeout is null || options.Timeout == Timeout.InfiniteTimeSpan)
        {
            options.Timeout = DefaultTimeout;
        }

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
            var stopwatch = Stopwatch.StartNew();

            RestResponse<T> response;
            try
            {
                response = await _client.ExecuteAsync<T>(request, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                stopwatch.Stop();
                throw new HttpRequestException($"{GetRequestName(request)} failed before receiving a response.", exception);
            }

            stopwatch.Stop();
            
            if (response.StatusCode != HttpStatusCode.TooManyRequests)
            {
                if (!response.IsSuccessful)
                {
                    var error = response.ErrorException?.Message
                                ?? response.ErrorMessage
                                ?? "No error message returned.";

                    throw new HttpRequestException(
                        $"{GetRequestName(request)} failed with status code {(int)response.StatusCode}: {error}");
                }

                if (response.Data is not null) return response.Data;
                throw new InvalidOperationException("The response contained no data.");

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

            if (delay > MaxRateLimitDelay)
            {
                throw new TimeoutException(
                    $"{GetRequestName(request)} was rate limited for longer than {MaxRateLimitDelay.TotalSeconds:N0} seconds.");
            }

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
}
