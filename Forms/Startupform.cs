namespace Pawductivity.Forms;

/// The very first screen the player sees.
/// Contains a single "Start Game" button that opens LoginForm.
public class StartupForm : Form
{
    private Button _btnStart = null!;

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
        //BackgroundImage = Image.FromFile("Assets/startup_bg.png");
        //BackgroundImageLayout = ImageLayout.Stretch;

        // ── Start Game button ─────────────────────────────────────────
        _btnStart = new Button
        {
            Text = "Start Game 🐾",
            Size = new Size(180, 48),
            Location = new Point(
                (ClientSize.Width - 180) / 2,
                (ClientSize.Height - 48) / 2),
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

        Controls.Add(_btnStart);
    }

    // ─────────────────────────────────────────────────────────────────
    // Opens LoginForm and hides this screen.
    // LoginForm will re-show this form if the user ever needs to
    // return (or you can just close it — up to you).
    // ─────────────────────────────────────────────────────────────────
    private void BtnStart_Click(object? sender, EventArgs e)
    {
        var login = new LoginForm();
        login.Show();
        Hide();

        // When the login window is fully closed (user quit without
        // logging in), close this startup screen too.
        login.FormClosed += (s, _) =>
        {
            if (!login.Tag?.Equals("launched") ?? true)
                Close();
        };
    }
}