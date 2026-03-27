namespace RJ_VC_Bypass;

public class StartForm : Form
{
    private Label tvStatus;
    private ProgressBar progress;
    private Panel layoutLogin;
    private TextBox etUser;
    private TextBox etKey;
    private Button btnStartJourney;
    private Button btnVerifyLicense;
    private Label lblTitle;
    private Label lblUser;
    private Label lblKey;

    private readonly HttpHelper _httpHelper;
    private string _currentDeviceId;

    public StartForm()
    {
        _httpHelper = new HttpHelper();
        _currentDeviceId = GetDeviceId();

        InitializeComponent();
        UpdateDeviceId();

        // Check if already logged in
        if (ConfigManager.Instance.IsLoggedIn)
        {
            OpenMainForm();
        }
    }

    /// <summary>
    /// Generate/get unique device ID
    /// </summary>
    private static string GetDeviceId()
    {
        // Use machine ID + user + volume serial for unique ID
        string machineName = Environment.MachineName;
        string userName = Environment.UserName;
        string osVersion = Environment.OSVersion.VersionString;

        string combined = $"{machineName}-{userName}-{osVersion}";
        return SecurityHelper.HashData(combined).Substring(0, 16).ToUpper();
    }

    private void UpdateDeviceId()
    {
        var deviceIdLabel = Controls.Find("tvDeviceId", true).FirstOrDefault() as Label;
        if (deviceIdLabel != null)
        {
            deviceIdLabel.Text = $"Device ID: {_currentDeviceId}";
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "RJ VC Bypass - Windows Edition";
        this.Size = new Size(500, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.White;

        // Title
        lblTitle = new Label
        {
            Text = "RJ VC Bypass",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(150, 40),
            ForeColor = Color.FromArgb(0, 229, 255)
        };
        this.Controls.Add(lblTitle);

        // Subtitle
        var lblSubtitle = new Label
        {
            Text = "Windows Edition v3.0",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(170, 85),
            ForeColor = Color.Gray
        };
        this.Controls.Add(lblSubtitle);

        // Device ID
        var tvDeviceId = new Label
        {
            Name = "tvDeviceId",
            Text = $"Device ID: {_currentDeviceId}",
            Font = new Font("Segoe UI", 9),
            AutoSize = true,
            Location = new Point(20, 130),
            ForeColor = Color.LightGray
        };
        this.Controls.Add(tvDeviceId);

        // Status label
        tvStatus = new Label
        {
            Text = "Checking Internet Connection...",
            Font = new Font("Segoe UI", 11),
            AutoSize = true,
            Location = new Point(20, 170),
            ForeColor = Color.FromArgb(0, 229, 255)
        };
        this.Controls.Add(tvStatus);

        // Progress bar
        progress = new ProgressBar
        {
            Location = new Point(20, 210),
            Size = new Size(440, 25),
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30
        };
        this.Controls.Add(progress);

        // Login panel (hidden initially)
        layoutLogin = new Panel
        {
            Location = new Point(20, 170),
            Size = new Size(440, 250),
            Visible = false,
            BackColor = Color.FromArgb(40, 40, 40)
        };

        lblUser = new Label
        {
            Text = "Username:",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 30),
            ForeColor = Color.LightGray
        };
        layoutLogin.Controls.Add(lblUser);

        etUser = new TextBox
        {
            Location = new Point(20, 55),
            Size = new Size(400, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        layoutLogin.Controls.Add(etUser);

        lblKey = new Label
        {
            Text = "License Key:",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 95),
            ForeColor = Color.LightGray
        };
        layoutLogin.Controls.Add(lblKey);

        etKey = new TextBox
        {
            Location = new Point(20, 120),
            Size = new Size(400, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = true
        };
        layoutLogin.Controls.Add(etKey);

        btnVerifyLicense = new Button
        {
            Text = "VERIFY & LOGIN",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(20, 165),
            Size = new Size(400, 45),
            BackColor = Color.FromArgb(0, 229, 255),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnVerifyLicense.FlatAppearance.BorderSize = 0;
        btnVerifyLicense.Click += BtnVerifyLicense_Click;
        layoutLogin.Controls.Add(btnVerifyLicense);

        this.Controls.Add(layoutLogin);

        // Start Journey Button
        btnStartJourney = new Button
        {
            Text = "START JOURNEY",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 300),
            Size = new Size(440, 50),
            BackColor = Color.FromArgb(0, 229, 255),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStartJourney.FlatAppearance.BorderSize = 0;
        btnStartJourney.Click += BtnStartJourney_Click;
        this.Controls.Add(btnStartJourney);

        // Footer
        var lblFooter = new Label
        {
            Text = "© 2024 RJ Bypass. All rights reserved.",
            Font = new Font("Segoe UI", 8),
            AutoSize = true,
            Location = new Point(140, 550),
            ForeColor = Color.Gray
        };
        this.Controls.Add(lblFooter);

        this.ResumeLayout(false);

        // Start connection check
        CheckAndBypassInternet();
    }

    private async void CheckAndBypassInternet()
    {
        progress.Visible = true;
        btnStartJourney.Visible = false;

        bool isInternetOk = false;
        while (!isInternetOk)
        {
            try
            {
                if (await _httpHelper.CheckInternetConnection())
                {
                    isInternetOk = true;
                    tvStatus.Text = "Internet Connected. Please Login.";
                    tvStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    progress.Visible = false;
                    layoutLogin.Visible = true;
                }
                else
                {
                    tvStatus.Text = "Captive Portal Detected. Bypassing...";
                    // Would start bypass service here
                    await Task.Delay(6000);
                }
            }
            catch (Exception ex)
            {
                tvStatus.Text = "Connection Lost. Retrying...";
                await Task.Delay(6000);
            }
        }
    }

    private async void BtnStartJourney_Click(object? sender, EventArgs e)
    {
        CheckAndBypassInternet();
    }

    private async void BtnVerifyLicense_Click(object? sender, EventArgs e)
    {
        string username = etUser.Text.Trim();
        string key = etKey.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(key))
        {
            MessageBox.Show("Please fill all fields!", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        tvStatus.Text = "Verifying License...";
        tvStatus.ForeColor = Color.FromArgb(0, 229, 255);
        progress.Visible = true;
        layoutLogin.Enabled = false;

        try
        {
            // Query Supabase for license
            string apiUrl = $"{AppConfig.SUPABASE_URL}/rest/v1/licenses?username=eq.{username}&select=*";

            using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("apikey", AppConfig.SUPABASE_API_KEY);
            request.Headers.Add("Authorization", $"Bearer {AppConfig.SUPABASE_ANON_KEY}");

            var response = await _httpHelper.ToString(); // Get raw response
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", AppConfig.SUPABASE_API_KEY);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SUPABASE_ANON_KEY}");

            var content = await httpClient.GetStringAsync(apiUrl);

            if (content.Contains("\"license_key\":\"" + key.Replace("\"", "\\\"") + "\"") ||
                content.Contains($"\"license_key\": \"{key}\""))
            {
                // Parse and check license
                if (content.Contains("\"is_banned\":true") || content.Contains("\"is_banned\": true"))
                {
                    MessageBox.Show("This license has been banned!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Extract expire date
                    string expireDate = "Unknown";
                    var expireMatch = System.Text.RegularExpressions.Regex.Match(content, "\"expire_date\":\"([^\"]+)\"");
                    if (expireMatch.Success)
                    {
                        expireDate = expireMatch.Groups[1].Value;
                    }

                    // Save credentials
                    ConfigManager.Instance.SavedUsername = username;
                    ConfigManager.Instance.SavedKey = key;
                    ConfigManager.Instance.ExpireDate = expireDate;
                    ConfigManager.Instance.DeviceId = _currentDeviceId;

                    MessageBox.Show($"Welcome {username}!\nLicense expires: {expireDate}", "Login Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    OpenMainForm();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or license key!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Network error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progress.Visible = false;
            layoutLogin.Enabled = true;
            tvStatus.Text = "Internet Connected. Please Login.";
        }
    }

    private void OpenMainForm()
    {
        this.Hide();
        var mainForm = new MainForm();
        mainForm.FormClosed += (s, e) => this.Close();
        mainForm.Show();
    }
}
