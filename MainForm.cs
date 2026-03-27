using System.Drawing;
using System.Text;

namespace RJ_VC_Bypass;

public class MainForm : Form
{
    private Label tvStatus;
    private TextBox tvTerminalLog;
    private Label tvUsername;
    private Label tvRemainingDays;
    private Button btnConnect;
    private Button btnStop;
    private Button btnBrute;
    private Button btnContact;
    private Button btnSpeedtest;
    private Button btnRedeem;

    private BypassEngine? _bypassEngine;
    private readonly ConfigManager _config;
    private readonly HttpHelper _httpHelper;

    public MainForm()
    {
        _config = ConfigManager.Instance;
        _httpHelper = new HttpHelper();

        InitializeComponent();
        LoadUserInfo();
        CheckLicenseStatus();

        // Initialize bypass engine
        _bypassEngine = new BypassEngine();
        _bypassEngine.LogReceived += BypassEngine_LogReceived;
        _bypassEngine.StatusReceived += BypassEngine_StatusReceived;
    }

    private void BypassEngine_LogReceived(object? sender, LogEventArgs e)
    {
        AppendLog(e.Message);
    }

    private void BypassEngine_StatusReceived(object? sender, StatusEventArgs e)
    {
        UpdateStatus(e.Status);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "RJ VC Bypass - Main Dashboard";
        this.Size = new Size(700, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.White;

        // Header with username
        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(40, 40, 40)
        };

        tvUsername = new Label
        {
            Text = "👤 User",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15),
            ForeColor = Color.FromArgb(0, 229, 255)
        };
        headerPanel.Controls.Add(tvUsername);

        tvRemainingDays = new Label
        {
            Text = "Loading...",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 50),
            ForeColor = Color.FromArgb(255, 193, 7)
        };
        headerPanel.Controls.Add(tvRemainingDays);

        this.Controls.Add(headerPanel);

        // Status panel
        var statusPanel = new Panel
        {
            Location = new Point(20, 100),
            Size = new Size(640, 60),
            BackColor = Color.FromArgb(50, 50, 50)
        };

        var lblStatusLabel = new Label
        {
            Text = "Status:",
            Font = new Font("Segoe UI", 11),
            AutoSize = true,
            Location = new Point(20, 15),
            ForeColor = Color.LightGray
        };
        statusPanel.Controls.Add(lblStatusLabel);

        tvStatus = new Label
        {
            Text = "DISCONNECTED",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(100, 12),
            ForeColor = Color.White
        };
        statusPanel.Controls.Add(tvStatus);

        this.Controls.Add(statusPanel);

        // Terminal log
        tvTerminalLog = new TextBox
        {
            Location = new Point(20, 180),
            Size = new Size(640, 300),
            Font = new Font("Consolas", 9),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(10, 10, 10),
            ForeColor = Color.FromArgb(0, 255, 0),
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(tvTerminalLog);

        // Button panel
        var buttonPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 500),
            Size = new Size(640, 120),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        // Connect Button
        btnConnect = new Button
        {
            Text = "CONNECT & BYPASS",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Size = new Size(140, 50),
            BackColor = Color.FromArgb(0, 229, 255),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnConnect.FlatAppearance.BorderSize = 0;
        btnConnect.Click += BtnConnect_Click;
        buttonPanel.Controls.Add(btnConnect);

        // Stop Button
        btnStop = new Button
        {
            Text = "STOP",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Size = new Size(100, 50),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStop.FlatAppearance.BorderSize = 0;
        btnStop.Click += BtnStop_Click;
        buttonPanel.Controls.Add(btnStop);

        // Brute Force Button
        btnBrute = new Button
        {
            Text = "BRUTE FORCE",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Size = new Size(120, 50),
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnBrute.FlatAppearance.BorderSize = 0;
        btnBrute.Click += BtnBrute_Click;
        buttonPanel.Controls.Add(btnBrute);

        // Contact Button
        btnContact = new Button
        {
            Text = "CONTACT",
            Font = new Font("Segoe UI", 10),
            Size = new Size(100, 50),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnContact.FlatAppearance.BorderSize = 0;
        btnContact.Click += BtnContact_Click;
        buttonPanel.Controls.Add(btnContact);

        // Speedtest Button
        btnSpeedtest = new Button
        {
            Text = "SPEEDTEST",
            Font = new Font("Segoe UI", 10),
            Size = new Size(100, 50),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSpeedtest.FlatAppearance.BorderSize = 0;
        btnSpeedtest.Click += BtnSpeedtest_Click;
        buttonPanel.Controls.Add(btnSpeedtest);

        // Redeem/Dashboard Button
        btnRedeem = new Button
        {
            Text = "DASHBOARD",
            Font = new Font("Segoe UI", 10),
            Size = new Size(100, 50),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRedeem.FlatAppearance.BorderSize = 0;
        btnRedeem.Click += BtnRedeem_Click;
        buttonPanel.Controls.Add(btnRedeem);

        this.Controls.Add(buttonPanel);

        // Footer
        var lblFooter = new Label
        {
            Text = "RJ VC Bypass v3.0 - Windows Edition",
            Font = new Font("Segoe UI", 8),
            AutoSize = true,
            Location = new Point(240, 650),
            ForeColor = Color.Gray
        };
        this.Controls.Add(lblFooter);

        this.ResumeLayout(false);
    }

    private void LoadUserInfo()
    {
        tvUsername.Text = $"👤 {_config.SavedUsername}";

        // Calculate remaining days
        if (!string.IsNullOrEmpty(_config.ExpireDate) && _config.ExpireDate.Length >= 10)
        {
            try
            {
                string expireOnly = _config.ExpireDate.Substring(0, 10);
                DateTime expireDate = DateTime.Parse(expireOnly);
                TimeSpan diff = expireDate - DateTime.Now;
                int daysRemaining = (int)diff.TotalDays;

                if (daysRemaining > 0)
                {
                    tvRemainingDays.Text = $"⏳ {daysRemaining} day{(daysRemaining > 1 ? "s" : "")} left";
                    tvRemainingDays.ForeColor = daysRemaining <= 3 ? Color.FromArgb(244, 67, 54) : Color.FromArgb(255, 193, 7);
                }
                else
                {
                    tvRemainingDays.Text = "⏳ EXPIRED!";
                    tvRemainingDays.ForeColor = Color.FromArgb(244, 67, 54);
                }
            }
            catch
            {
                tvRemainingDays.Text = "⏳ Unknown";
            }
        }
    }

    private async void CheckLicenseStatus()
    {
        // Sync with server
        AppendLog("⏳ Verifying license with server...");

        try
        {
            string apiUrl = $"{AppConfig.SUPABASE_URL}/rest/v1/licenses?username=eq.{_config.SavedUsername}&select=*";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", AppConfig.SUPABASE_API_KEY);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SUPABASE_ANON_KEY}");

            string content = await httpClient.GetStringAsync(apiUrl);

            if (content.Contains("\"is_banned\":true") || content.Contains("\"is_banned\": true"))
            {
                AppendLog("❌ License has been banned by Admin!");
                MessageBox.Show("Your license has been banned!", "Banned",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logout();
                return;
            }

            // Extract new expire date
            var expireMatch = System.Text.RegularExpressions.Regex.Match(content, "\"expire_date\":\"([^\"]+)\"");
            if (expireMatch.Success)
            {
                string newExpireDate = expireMatch.Groups[1].Value;
                _config.ExpireDate = newExpireDate;

                // Check if expired
                if (newExpireDate.Length >= 10)
                {
                    string expireOnly = newExpireDate.Substring(0, 10);
                    string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                    if (string.Compare(currentDate, expireOnly) > 0)
                    {
                        AppendLog("❌ License Expired!");
                        MessageBox.Show("Your license has expired! Please contact admin to renew.", "Expired",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Logout();
                        return;
                    }
                }
            }

            AppendLog("✅ License verified successfully!");
        }
        catch (Exception ex)
        {
            AppendLog($"> License check error: {ex.Message}");
        }
    }

    private void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (_bypassEngine == null || _bypassEngine.IsRunning) return;

        btnConnect.Text = "CONNECTING...";
        btnConnect.BackColor = Color.FromArgb(0, 229, 255);
        tvTerminalLog.Clear();
        UpdateStatus("CONNECTING");

        AppendLog("Initializing Bypass Engine...");
        _bypassEngine.Start();
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (_bypassEngine == null || !_bypassEngine.IsRunning) return;

        _bypassEngine.Stop();
        UpdateStatus("DISCONNECTED");
        btnConnect.Text = "CONNECT & BYPASS";
        btnConnect.BackColor = Color.FromArgb(0, 229, 255);
        AppendLog("Process Stopped by User.");
        MessageBox.Show("Service Stopped", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnBrute_Click(object? sender, EventArgs e)
    {
        // Open brute force form
        var bruteForm = new BruteForceForm();
        bruteForm.ShowDialog();
    }

    private void BtnContact_Click(object? sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://t.me/injectionvoucher",
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show("Could not open browser. Please visit: https://t.me/injectionvoucher", "Contact",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnSpeedtest_Click(object? sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.speedtest.net",
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show("Could not open browser.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRedeem_Click(object? sender, EventArgs e)
    {
        var dashboardForm = new DashboardForm();
        dashboardForm.ShowDialog();
    }

    private void AppendLog(string message)
    {
        if (tvTerminalLog.InvokeRequired)
        {
            tvTerminalLog.Invoke(new Action(() => AppendLog(message)));
            return;
        }

        // Color-code messages
        string coloredMessage = message + Environment.NewLine;
        if (message.Contains("❌") || message.Contains("Error") || message.Contains("Fail"))
        {
            tvTerminalLog.ForeColor = Color.FromArgb(244, 67, 54);
        }
        else if (message.Contains("✅") || message.Contains("SUCCESS") || message.Contains("🌐"))
        {
            tvTerminalLog.ForeColor = Color.FromArgb(76, 175, 80);
        }
        else if (message.Contains("⏳") || message.Contains("Waiting"))
        {
            tvTerminalLog.ForeColor = Color.FromArgb(255, 193, 7);
        }
        else if (message.Contains(">") || message.Contains("🚀"))
        {
            tvTerminalLog.ForeColor = Color.FromArgb(0, 229, 255);
        }
        else
        {
            tvTerminalLog.ForeColor = Color.FromArgb(0, 255, 0);
        }

        tvTerminalLog.AppendText(coloredMessage);
        tvTerminalLog.ScrollToCaret();
    }

    private void UpdateStatus(string status)
    {
        if (tvStatus.InvokeRequired)
        {
            tvStatus.Invoke(new Action(() => UpdateStatus(status)));
            return;
        }

        tvStatus.Text = status;
        tvStatus.ForeColor = status switch
        {
            "CONNECTED" => Color.FromArgb(76, 175, 80),
            "CONNECTING" => Color.FromArgb(0, 229, 255),
            "RECONNECTING" => Color.FromArgb(255, 152, 0),
            "BYPASSING" => Color.FromArgb(0, 229, 255),
            _ => Color.White
        };
    }

    private void Logout()
    {
        _config.Clear();
        this.Hide();
        var startForm = new StartForm();
        startForm.FormClosed += (s, e) => Application.Exit();
        startForm.Show();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _bypassEngine?.Stop();
        _bypassEngine?.Dispose();
        base.OnFormClosing(e);
    }
}
