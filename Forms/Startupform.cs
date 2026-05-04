namespace Pawductivity.Forms;

public class StartupForm : Form
{
    private Button _btnStart = null!;
    private Button _btnQuit = null!;
    private Label _lblTip = null!;
    private System.Windows.Forms.Timer _tipTimer = null!;

    private static readonly string[] _tips =
    [
        "🐾 Complete tasks on time to keep your pet happy!",
        "💖 Your pet loses mood when tasks go overdue.",
        "⭐ Gain XP by finishing tasks before the deadline.",
        "🛒 Spend coins in the shop to get cool items for your pet!",
        "😴 Don't forget — your pet needs you to stay productive!",
        "🎉 Level up your pet by completing tasks consistently.",
        "❤️ A healthy pet means a productive you!",
        "🐱 Cats and dogs both love it when you finish your to-do list.",
        "💰 The more tasks you complete, the more coins you earn.",
        "🌟 Keep your streak going to evolve your pet faster!",
    ];

    private int _currentTip = 0;

    public StartupForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // ── Window ────────────────────────────────────────────────────
        Text = "Pawductivity 🐾";
        ClientSize = new Size(1024, 768);
        MinimumSize = new Size(1024, 768);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // ── Background image ──────────────────────────────────────────
        BackgroundImage = Image.FromFile("Assets/startup_bg.png");
        BackgroundImageLayout = ImageLayout.Stretch;

        // ── Shared position values ────────────────────────────────────
        int btnX = ClientSize.Width - 250 - 187;
        int btnY = (ClientSize.Height - 50) / 2;
        int btnGap = 16;

        // ── Start Game button ─────────────────────────────────────────
        _btnStart = new Button
        {
            Text = "Start Game 🐾",
            Size = new Size(250, 50),
            Location = new Point(btnX, btnY),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(255, 105, 150),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        _btnStart.FlatAppearance.BorderSize = 0;
        _btnStart.MouseEnter += (s, e) =>
            _btnStart.BackColor = Color.FromArgb(240, 85, 130);
        _btnStart.MouseLeave += (s, e) =>
            _btnStart.BackColor = Color.FromArgb(255, 105, 150);
        _btnStart.Click += BtnStart_Click;

        // ── Quit button ───────────────────────────────────────────────
        _btnQuit = new Button
        {
            Text = "Quit",
            Size = new Size(250, 50),
            Location = new Point(btnX, btnY + 50 + btnGap),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(255, 105, 150),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        _btnQuit.FlatAppearance.BorderSize = 0;
        _btnQuit.MouseEnter += (s, e) =>
            _btnQuit.BackColor = Color.FromArgb(240, 85, 130);
        _btnQuit.MouseLeave += (s, e) =>
            _btnQuit.BackColor = Color.FromArgb(255, 105, 150);
        _btnQuit.Click += (s, e) => Application.Exit();

        // ── Tip label ─────────────────────────────────────────────────
        _lblTip = new Label
        {
            Text = _tips[0],
            AutoSize = false,
            Size = new Size(ClientSize.Width - 40, 40),
            Location = new Point(20, ClientSize.Height - 60),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(255, 210, 220),   
            ForeColor = Color.FromArgb(180, 80, 100),    
            Font = new Font("Segoe UI", 10f, FontStyle.Italic),
        };

        // ── Tip timer — rotates every 4 seconds ───────────────────────
        _tipTimer = new System.Windows.Forms.Timer
        {
            Interval = 4000,
        };
        _tipTimer.Tick += (s, e) =>
        {
            _currentTip = (_currentTip + 1) % _tips.Length;
            _lblTip.Text = _tips[_currentTip];
        };
        _tipTimer.Start();

        Controls.Add(_btnStart);
        Controls.Add(_btnQuit);
        Controls.Add(_lblTip);
    }

    // ─────────────────────────────────────────────────────────────────
    // Opens LoginForm and hides this screen.
    // ─────────────────────────────────────────────────────────────────
    private void BtnStart_Click(object? sender, EventArgs e)
    {
        _tipTimer.Stop();

        var login = new LoginForm();
        login.Show();
        Hide();

        login.FormClosed += (s, _) =>
        {
            if (!login.Tag?.Equals("launched") ?? true)
                Close();
        };
    }
}
