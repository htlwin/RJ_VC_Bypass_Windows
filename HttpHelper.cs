using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RJ_VC_Bypass;

/// <summary>
/// HTTP Client helper with SSL bypass and cookie management
/// </summary>
public class HttpHelper
{
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _client;
    private readonly CookieContainer _cookieContainer;

    public HttpHelper()
    {
        _cookieContainer = new CookieContainer();

        _handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        _client = new HttpClient(_handler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _client.DefaultRequestHeaders.Add("User-Agent", AppConfig.USER_AGENT);
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
    }

    /// <summary>
    /// GET request
    /// </summary>
    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await _client.GetAsync(url);
    }

    /// <summary>
    /// POST JSON request
    /// </summary>
    public async Task<HttpResponseMessage> PostJsonAsync(string url, string json)
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _client.PostAsync(url, content);
    }

    /// <summary>
    /// GET request with custom headers
    /// </summary>
    public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return await _client.SendAsync(request);
    }

    /// <summary>
    /// Check if internet is connected (via Google connectivity check)
    /// </summary>
    public async Task<bool> CheckInternetConnection()
    {
        try
        {
            var response = await _client.GetAsync("http://connectivitycheck.gstatic.com/generate_204");
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get final response URL after redirects
    /// </summary>
    public async Task<string> GetFinalUrlAsync(string url)
    {
        try
        {
            var response = await _client.GetAsync(url);
            return response.RequestMessage?.RequestUri?.ToString() ?? url;
        }
        catch
        {
            return url;
        }
    }

    /// <summary>
    /// Get page content
    /// </summary>
    public async Task<string> GetStringAsync(string url)
    {
        return await _client.GetStringAsync(url);
    }
}
