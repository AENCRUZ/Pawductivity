using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pawductivity.Animations;

public class FloatyLabel : Label
{
    public static readonly Color XpColor = Color.FromArgb(80, 160, 255);
    public static readonly Color MoodColor = Color.FromArgb(220, 160, 40);
    public static readonly Color HealthColor = Color.FromArgb(220, 60, 100);
    public static readonly Color CoinColor = Color.FromArgb(200, 160, 0);
    public static readonly Color DmgColor = Color.FromArgb(200, 40, 40);

    private float _alpha = 1f;
    private float _dy;
    private readonly System.Windows.Forms.Timer _t = new() { Interval = 16 };

    private FloatyLabel(string text, Color color)
    {
        Text = text;
        ForeColor = color;
        AutoSize = true;
        Font = new Font("Segoe UI Emoji", 9f, FontStyle.Bold);

        BackColor = Color.Transparent;

        SetStyle(
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint,
            true);

        _t.Tick += Tick;
    }

    /// Spawns a floating label over <paramref name="anchor"/> on
    /// <paramref name="owner"/>. The label rises ~40 px and self-destructs.
    public static void Show(Form owner, Control anchor, string text, Color color)
    {
        var lbl = new FloatyLabel(text, color);

        var pt = owner.PointToClient(
            anchor.PointToScreen(new Point(anchor.Width / 2, -4)));

        lbl.Location = new Point(pt.X - 20, pt.Y);
        owner.Controls.Add(lbl);
        lbl.BringToFront();
        lbl._t.Start();
    }

    private void Tick(object? s, EventArgs e)
    {
        _dy += 0.8f;
        _alpha -= 0.018f;
        Top -= (int)_dy / 8;

        if (_alpha <= 0)
        {
            _t.Stop();
            _t.Dispose();
            Parent?.Controls.Remove(this);
            Dispose();
            return;
        }

        Invalidate();
    }

    // ── Paint: draw text with fading alpha, no background fill at all ────────
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Intentionally empty — let whatever is behind show through.
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        int a = (int)Math.Clamp(_alpha * 255, 0, 255);
        using var brush = new SolidBrush(Color.FromArgb(a, ForeColor));
        e.Graphics.DrawString(Text, Font, brush, PointF.Empty);
    }
}