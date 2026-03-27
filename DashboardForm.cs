namespace RJ_VC_Bypass;

public class DashboardForm : Form
{
    private Label tvUsername;
    private Label tvExpireDate;
    private Label tvRemainingDays;
    private Button btnContactAdmin;
    private Button btnLogout;
    private readonly ConfigManager _config;

    public DashboardForm()
    {
        _config = ConfigManager.Instance;
        InitializeComponent();
        LoadLicenseInfo();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "RJ VC Bypass - Dashboard";
        this.Size = new Size(500, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.White;

        // Title
        var lblTitle = new Label
        {
            Text = "📋 License Dashboard",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(150, 30),
            ForeColor = Color.FromArgb(255, 152, 0)
        };
        this.Controls.Add(lblTitle);

        // Info panel
        var infoPanel = new Panel
        {
            Location = new Point(50, 100),
            Size = new Size(400, 200),
            BackColor = Color.FromArgb(40, 40, 40)
        };

        tvUsername = new Label
        {
            Text = "Username: Unknown",
            Font = new Font("Segoe UI", 12),
            AutoSize = true,
            Location = new Point(30, 30),
            ForeColor = Color.White
        };
        infoPanel.Controls.Add(tvUsername);

        tvExpireDate = new Label
        {
            Text = "Expire Date: Unknown",
            Font = new Font("Segoe UI", 12),
            AutoSize = true,
            Location = new Point(30, 70),
            ForeColor = Color.White
        };
        infoPanel.Controls.Add(tvExpireDate);

        tvRemainingDays = new Label
        {
            Text = "⏳ Remaining: Unknown",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(30, 120),
            ForeColor = Color.FromArgb(255, 193, 7)
        };
        infoPanel.Controls.Add(tvRemainingDays);

        this.Controls.Add(infoPanel);

        // Contact Admin Button
        btnContactAdmin = new Button
        {
            Text = "📨 Contact Admin",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(50, 330),
            Size = new Size(400, 50),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnContactAdmin.FlatAppearance.BorderSize = 0;
        btnContactAdmin.Click += BtnContactAdmin_Click;
        this.Controls.Add(btnContactAdmin);

        // Logout Button
        btnLogout = new Button
        {
            Text = "🚪 Logout",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(50, 400),
            Size = new Size(400, 50),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += BtnLogout_Click;
        this.Controls.Add(btnLogout);

        this.ResumeLayout(false);
    }

    private void LoadLicenseInfo()
    {
        tvUsername.Text = $"Username: {_config.SavedUsername}";
        tvExpireDate.Text = $"Expire Date: {_config.ExpireDate}";

        if (!string.IsNullOrEmpty(_config.ExpireDate) && _config.ExpireDate.Length >= 10)
        {
            try
            {
                string expireOnly = _config.ExpireDate.Substring(0, 10);
                DateTime expireDate = DateTime.Parse(expireOnly);
                TimeSpan diff = expireDate - DateTime.Now;
                int daysRemaining = (int)diff.TotalDays;

                if (daysRemaining >= 0)
                {
                    tvRemainingDays.Text = $"⏳ Remaining: {daysRemaining} Days";
                    tvRemainingDays.ForeColor = daysRemaining <= 3 ?
                        Color.FromArgb(244, 67, 54) : Color.FromArgb(76, 175, 80);
                }
                else
                {
                    tvRemainingDays.Text = "⏳ Remaining: EXPIRED!";
                    tvRemainingDays.ForeColor = Color.FromArgb(244, 67, 54);
                }
            }
            catch
            {
                tvRemainingDays.Text = "⏳ Remaining: Unknown";
            }
        }
    }

    private void BtnContactAdmin_Click(object? sender, EventArgs e)
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
            MessageBox.Show("Could not open browser. Please visit: https://t.me/injectionvoucher",
                "Contact Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to logout?\n\nYou will need to enter your license key again to continue.",
            "Confirm Logout",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _config.Clear();
            this.Hide();

            var startForm = new StartForm();
            startForm.FormClosed += (s, args) => Application.Exit();
            startForm.Show();
        }
    }
}
