namespace Pawductivity.Forms;

public class StartupForm : Form
{
    private Button _btnStart = null!;
    private Button _btnQuit = null!;

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
        int btnGap = 16;   // space between the two buttons

        // ── Start Game button ─────────────────────────────────────────
        _btnStart = new Button
        {
            Text = "Start Game 🐾",
            Size = new Size(250, 50),
            Location = new Point(btnX, btnY),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(255, 105, 150),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
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
            Location = new Point(btnX, btnY + 50 + btnGap),  // right below Start
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(255, 105, 150),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        _btnQuit.FlatAppearance.BorderSize = 0;
        _btnQuit.MouseEnter += (s, e) =>
            _btnQuit.BackColor = Color.FromArgb(240, 85, 130);
        _btnQuit.MouseLeave += (s, e) =>
            _btnQuit.BackColor = Color.FromArgb(255, 105, 150);
        _btnQuit.Click += (s, e) => Application.Exit();

        Controls.Add(_btnStart);
        Controls.Add(_btnQuit);
    }

    private void BtnStart_Click(object? sender, EventArgs e)
    {
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
