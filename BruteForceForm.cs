using System.Text;

namespace RJ_VC_Bypass;

public class BruteForceForm : Form
{
    private Label tvAttackStatus;
    private Label tvKeyProgress;
    private Label tvKeyAttempts;
    private Label tvKeySpeed;
    private TextBox tvAttackLog;
    private TextBox tvSuccessBox;
    private TextBox etStartKey;
    private TextBox etEndKey;
    private ProgressBar progressAttack;
    private Label tvFoundCount;
    private Button btnStart;
    private Button btnStop;
    private Button btnClearLog;
    private Button btnViewSuccess;

    private BruteForceEngine? _engine;
    private readonly ConfigManager _config;

    public BruteForceForm()
    {
        _config = ConfigManager.Instance;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "RJ VC Bypass - Key Brute Force";
        this.Size = new Size(800, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.White;

        // Title
        var lblTitle = new Label
        {
            Text = "🔑 Key Brute Force Attack",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20),
            ForeColor = Color.FromArgb(156, 39, 176)
        };
        this.Controls.Add(lblTitle);

        // Status
        tvAttackStatus = new Label
        {
            Text = "Ready to Attack",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 65),
            ForeColor = Color.FromArgb(255, 165, 0)
        };
        this.Controls.Add(tvAttackStatus);

        // Range input panel
        var rangePanel = new Panel
        {
            Location = new Point(20, 110),
            Size = new Size(740, 80),
            BackColor = Color.FromArgb(40, 40, 40)
        };

        var lblStart = new Label
        {
            Text = "Start Code:",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        rangePanel.Controls.Add(lblStart);

        etStartKey = new TextBox
        {
            Location = new Point(20, 40),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Text = "000000"
        };
        rangePanel.Controls.Add(etStartKey);

        var lblEnd = new Label
        {
            Text = "End Code:",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(200, 15)
        };
        rangePanel.Controls.Add(lblEnd);

        etEndKey = new TextBox
        {
            Location = new Point(200, 40),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Text = "999999"
        };
        rangePanel.Controls.Add(etEndKey);

        var lblSid = new Label
        {
            Text = "Using Captured SID from main service",
            Font = new Font("Segoe UI", 9),
            AutoSize = true,
            Location = new Point(400, 20),
            ForeColor = Color.Gray
        };
        rangePanel.Controls.Add(lblSid);

        this.Controls.Add(rangePanel);

        // Progress section
        var progressPanel = new Panel
        {
            Location = new Point(20, 210),
            Size = new Size(740, 100),
            BackColor = Color.FromArgb(40, 40, 40)
        };

        progressAttack = new ProgressBar
        {
            Location = new Point(20, 20),
            Size = new Size(700, 25),
            Style = ProgressBarStyle.Continuous
        };
        progressPanel.Controls.Add(progressAttack);

        tvKeyProgress = new Label
        {
            Text = "0%",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(350, 50),
            ForeColor = Color.FromArgb(0, 229, 255)
        };
        progressPanel.Controls.Add(tvKeyProgress);

        tvKeyAttempts = new Label
        {
            Text = "📊 0",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 55),
            ForeColor = Color.LightGray
        };
        progressPanel.Controls.Add(tvKeyAttempts);

        tvKeySpeed = new Label
        {
            Text = "⚡ Speed: 0 codes/s",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(200, 55),
            ForeColor = Color.LightGray
        };
        progressPanel.Controls.Add(tvKeySpeed);

        tvFoundCount = new Label
        {
            Text = "🏆 Found: 0",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(400, 55),
            ForeColor = Color.FromArgb(76, 175, 80)
        };
        progressPanel.Controls.Add(tvFoundCount);

        this.Controls.Add(progressPanel);

        // Terminal log
        tvAttackLog = new TextBox
        {
            Location = new Point(20, 330),
            Size = new Size(500, 250),
            Font = new Font("Consolas", 9),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(10, 10, 10),
            ForeColor = Color.FromArgb(0, 255, 0)
        };
        this.Controls.Add(tvAttackLog);

        // Success box
        tvSuccessBox = new TextBox
        {
            Location = new Point(540, 330),
            Size = new Size(220, 250),
            Font = new Font("Consolas", 9),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(10, 30, 10),
            ForeColor = Color.FromArgb(76, 175, 80)
        };
        this.Controls.Add(tvSuccessBox);

        var lblSuccessTitle = new Label
        {
            Text = "🏆 Found Keys:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(540, 310),
            ForeColor = Color.FromArgb(76, 175, 80)
        };
        this.Controls.Add(lblSuccessTitle);

        // Button panel
        var buttonPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 600),
            Size = new Size(740, 60),
            FlowDirection = FlowDirection.LeftToRight
        };

        btnStart = new Button
        {
            Text = "▶ START ATTACK",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(150, 45),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += BtnStart_Click;
        buttonPanel.Controls.Add(btnStart);

        btnStop = new Button
        {
            Text = "⏹ STOP",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(120, 45),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStop.FlatAppearance.BorderSize = 0;
        btnStop.Click += BtnStop_Click;
        buttonPanel.Controls.Add(btnStop);

        btnClearLog = new Button
        {
            Text = "🗑 CLEAR LOG",
            Font = new Font("Segoe UI", 10),
            Size = new Size(120, 45),
            BackColor = Color.FromArgb(158, 158, 158),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnClearLog.FlatAppearance.BorderSize = 0;
        btnClearLog.Click += BtnClearLog_Click;
        buttonPanel.Controls.Add(btnClearLog);

        btnViewSuccess = new Button
        {
            Text = "📋 VIEW HISTORY",
            Font = new Font("Segoe UI", 10),
            Size = new Size(140, 45),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnViewSuccess.FlatAppearance.BorderSize = 0;
        btnViewSuccess.Click += BtnViewSuccess_Click;
        buttonPanel.Controls.Add(btnViewSuccess);

        this.Controls.Add(buttonPanel);

        this.ResumeLayout(false);
    }

    private void BtnStart_Click(object? sender, EventArgs e)
    {
        if (_engine != null && _engine.IsRunning) return;

        // Parse range
        if (!int.TryParse(etStartKey.Text, out int start) || !int.TryParse(etEndKey.Text, out int end))
        {
            MessageBox.Show("Please enter valid numbers!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (start > end)
        {
            MessageBox.Show("Start code must be less than end code!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Get saved SID
        string sessionId = BypassEngine.LoadSavedSid();
        if (string.IsNullOrEmpty(sessionId))
        {
            MessageBox.Show("No Captured SID found! Please connect to the portal first from the main screen.",
                "No SID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Initialize engine
        _engine = new BruteForceEngine();
        _engine.LogMessage += Engine_LogMessage;
        _engine.KeyFound += Engine_KeyFound;
        _engine.ProgressUpdate += Engine_ProgressUpdate;

        tvAttackStatus.Text = "Checking SID...";
        tvAttackStatus.ForeColor = Color.FromArgb(255, 165, 0);
        tvAttackLog.Clear();
        tvSuccessBox.Clear();

        AppendLog($"> Using Captured SID: {sessionId}");
        AppendLog("> Launching Attack Threads...");

        _engine.StartAttack(start, end, sessionId);
        _engine.StartProgressUpdater();
    }

    private void Engine_ProgressUpdate(object? sender, BruteForceProgressEventArgs e)
    {
        if (tvKeyProgress.InvokeRequired)
        {
            tvKeyProgress.Invoke(new Action(() => UpdateProgress(e)));
            return;
        }

        int percent = e.TotalKeys > 0 ? (e.TestedCount * 100) / e.TotalKeys : 0;
        progressAttack.Value = Math.Min(percent, 100);
        tvKeyProgress.Text = $"{percent}%";
        tvKeyAttempts.Text = $"📊 {e.TestedCount}";
        tvKeySpeed.Text = $"⚡ Speed: {e.Speed} codes/s";
        tvFoundCount.Text = $"🏆 Found: {e.FoundCount}";
        tvAttackStatus.Text = $"Testing: {e.CurrentCode}";
        tvAttackStatus.ForeColor = Color.FromArgb(0, 229, 255);
    }

    private void Engine_LogMessage(object? sender, string message)
    {
        AppendLog(message);
    }

    private void Engine_KeyFound(object? sender, string key)
    {
        if (tvSuccessBox.InvokeRequired)
        {
            tvSuccessBox.Invoke(new Action(() => tvSuccessBox.AppendText($"✅ {key}{Environment.NewLine}")));
            return;
        }
        tvSuccessBox.AppendText($"✅ {key}{Environment.NewLine}");
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (_engine == null || !_engine.IsRunning) return;

        _engine.Stop();
        tvAttackStatus.Text = "🛑 Stopped";
        tvAttackStatus.ForeColor = Color.FromArgb(244, 67, 54);
        AppendLog("> 🛑 Process Stopped by User.");
        MessageBox.Show("🛑 Attack Stopped", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnClearLog_Click(object? sender, EventArgs e)
    {
        tvAttackLog.Clear();
        AppendLog("> Log cleared...");
        MessageBox.Show("Log cleared", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnViewSuccess_Click(object? sender, EventArgs e)
    {
        if (_engine == null || _engine.FoundKeys.Count == 0)
        {
            MessageBox.Show("No keys found yet!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder("🏆 Found Keys History:\n\n");
        foreach (var key in _engine.FoundKeys)
        {
            sb.Append($"✅ {key}\n");
        }
        tvAttackLog.Text = sb.ToString();
    }

    private void AppendLog(string message)
    {
        if (tvAttackLog.InvokeRequired)
        {
            tvAttackLog.Invoke(new Action(() => AppendLog(message)));
            return;
        }

        tvAttackLog.AppendText(message + Environment.NewLine);
        tvAttackLog.ScrollToCaret();
    }

    private void UpdateProgress(BruteForceProgressEventArgs e)
    {
        int percent = e.TotalKeys > 0 ? (e.TestedCount * 100) / e.TotalKeys : 0;
        progressAttack.Value = Math.Min(percent, 100);
        tvKeyProgress.Text = $"{percent}%";
        tvKeyAttempts.Text = $"📊 {e.TestedCount}";
        tvKeySpeed.Text = $"⚡ Speed: {e.Speed} codes/s";
        tvFoundCount.Text = $"🏆 Found: {e.FoundCount}";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _engine?.Stop();
        base.OnFormClosing(e);
    }
}
